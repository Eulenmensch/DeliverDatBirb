using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HBAORendererFeature : ScriptableRendererFeature
{
    public enum Quality
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }

    public enum Resolution
    {
        Full,
        Half
    }

    public enum DisplayMode
    {
        Normal,
        AOOnly,
        ColorBleedingOnly,
        SplitWithoutAOAndWithAO,
        SplitWithAOAndAOOnly,
        SplitWithoutAOAndAOOnly,
        ViewNormals
    }

    public enum Blur
    {
        None,
        Narrow,
        Medium,
        Wide,
        ExtraWide
    }

    public enum PerPixelNormals
    {
        //Camera,
        Reconstruct2Samples,
        Reconstruct4Samples
    }

    private class HBAORenderPass : ScriptableRenderPass
    {
        public HBAORendererSettings settings;

        private Material hbaoMaterial { get; set; }
        private RenderTargetIdentifier source { get; set; }
        private Texture2D m_NoiseTex;
        private Quality m_Quality;
        private static readonly int[] m_NumSampleDirections = new int[] { 3, 4, 6, 8, 8 }; // LOWEST, LOW, MEDIUM, HIGH, HIGHEST (highest uses more steps)

        private static class MersenneTwister
        {
            // Mersenne-Twister random numbers in [0,1).
            public static float[] Numbers = new float[] {
                0.463937f,0.340042f,0.223035f,0.468465f,0.322224f,0.979269f,0.031798f,0.973392f,0.778313f,0.456168f,0.258593f,0.330083f,0.387332f,0.380117f,0.179842f,0.910755f,
                0.511623f,0.092933f,0.180794f,0.620153f,0.101348f,0.556342f,0.642479f,0.442008f,0.215115f,0.475218f,0.157357f,0.568868f,0.501241f,0.629229f,0.699218f,0.707733f
            };
        }

        private static class Pass
        {
            public const int AO_LowestQuality = 0;
            public const int AO_LowQuality = 1;
            public const int AO_MediumQuality = 2;
            public const int AO_HighQuality = 3;
            public const int AO_HighestQuality = 4;
            public const int Blur_X_Narrow = 5;
            public const int Blur_X_Medium = 6;
            public const int Blur_X_Wide = 7;
            public const int Blur_X_ExtraWide = 8;
            public const int Blur_Y_Narrow = 9;
            public const int Blur_Y_Medium = 10;
            public const int Blur_Y_Wide = 11;
            public const int Blur_Y_ExtraWide = 12;
            public const int Composite = 13;
            public const int Composite_MultiBounce = 14;
            public const int Debug_AO_Only = 15;
            public const int Debug_ColorBleeding_Only = 16;
            public const int Debug_Split_WithoutAO_WithAO = 17;
            public const int Debug_Split_WithAO_AOOnly = 18;
            public const int Debug_Split_WithoutAO_AOOnly = 19;
            public const int Debug_ViewNormals = 20;
        }

        private static class ShaderProperties
        {
            public static int inputTex;
            public static int hbaoTex;
            public static int tempTex;
            public static int noiseTex;
            public static int screenSize;
            public static int uvToView;
            public static int radius;
            public static int maxRadiusPixels;
            public static int negInvRadius2;
            public static int angleBias;
            public static int aoMultiplier;
            public static int intensity;
            public static int multiBounceInfluence;
            public static int offscreenSamplesContrib;
            public static int maxDistance;
            public static int distanceFalloff;
            public static int baseColor;
            public static int targetScale;
            public static int blurSharpness;
            public static int colorBleedSaturation;
            public static int colorBleedBrightnessMask;
            public static int colorBleedBrightnessMaskRange;

            static ShaderProperties()
            {
                inputTex = Shader.PropertyToID("_InputTex");
                hbaoTex = Shader.PropertyToID("_HBAOTex");
                tempTex = Shader.PropertyToID("_TempTex");
                noiseTex = Shader.PropertyToID("_NoiseTex");
                screenSize = Shader.PropertyToID("_ScreenSize");
                uvToView = Shader.PropertyToID("_UVToView");
                radius = Shader.PropertyToID("_Radius");
                maxRadiusPixels = Shader.PropertyToID("_MaxRadiusPixels");
                negInvRadius2 = Shader.PropertyToID("_NegInvRadius2");
                angleBias = Shader.PropertyToID("_AngleBias");
                aoMultiplier = Shader.PropertyToID("_AOmultiplier");
                intensity = Shader.PropertyToID("_Intensity");
                multiBounceInfluence = Shader.PropertyToID("_MultiBounceInfluence");
                offscreenSamplesContrib = Shader.PropertyToID("_OffscreenSamplesContrib");
                maxDistance = Shader.PropertyToID("_MaxDistance");
                distanceFalloff = Shader.PropertyToID("_DistanceFalloff");
                baseColor = Shader.PropertyToID("_BaseColor");
                targetScale = Shader.PropertyToID("_TargetScale");
                blurSharpness = Shader.PropertyToID("_BlurSharpness");
                colorBleedSaturation = Shader.PropertyToID("_ColorBleedSaturation");
                colorBleedBrightnessMask = Shader.PropertyToID("_ColorBleedBrightnessMask");
                colorBleedBrightnessMaskRange = Shader.PropertyToID("_ColorBleedBrightnessMaskRange");
            }
        }

        public void Setup(Material material, RenderTargetIdentifier source)
        {
            this.hbaoMaterial = material;
            this.source = source;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var opaqueDesc = cameraTextureDescriptor;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(ShaderProperties.inputTex, opaqueDesc);

            var aoScale = settings.resolution == Resolution.Full ? Vector2.one : new Vector2(0.5f, 0.5f);
            //var blurScale = aoScale * (settings.blurDownsample ? new Vector2(0.5f, 0.5f) : Vector2.one);

            var aoRTDesc = new RenderTextureDescriptor(
                (int)(cameraTextureDescriptor.width * aoScale.x), (int)(cameraTextureDescriptor.height * aoScale.y),
                colorFormat: RenderTextureFormat.Default, 
                depthBufferBits: 0
            );

            /*var tempRTDesc = new RenderTextureDescriptor(
                (int)(cameraTextureDescriptor.width * blurScale.x), (int)(cameraTextureDescriptor.height * blurScale.y),
                (settings.colorBleedingEnabled || settings.displayMode == DisplayMode.ViewNormals) ? RenderTextureFormat.Default : RenderTextureFormat.RFloat,
                depthBufferBits: 0
            );*/

            cmd.GetTemporaryRT(ShaderProperties.hbaoTex, aoRTDesc);
            if (settings.blurType != Blur.None)
                cmd.GetTemporaryRT(ShaderProperties.tempTex, aoRTDesc);

            //ConfigureTarget(ShaderProperties.hbaoTex);
            //ConfigureClear(ClearFlag.Color, Color.white);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (hbaoMaterial == null)
            {
                Debug.LogError("HBAO material has not been correctly initialized...");
                return;
            }

            UpdateMaterialProperties(renderingData);

            var cmd = CommandBufferPool.Get("HBAO");

            CoreUtils.SetRenderTarget(cmd, ShaderProperties.hbaoTex);
            CoreUtils.ClearRenderTarget(cmd, ClearFlag.Color, Color.white);

            Blit(cmd, source, ShaderProperties.inputTex);
            Blit(cmd, ShaderProperties.inputTex, ShaderProperties.hbaoTex, hbaoMaterial, passIndex: GetAoPassId());

            if (settings.blurType != Blur.None)
            {
                Blit(cmd, ShaderProperties.hbaoTex, ShaderProperties.tempTex, hbaoMaterial, passIndex: GetBlurXPassId());
                Blit(cmd, ShaderProperties.tempTex, ShaderProperties.hbaoTex, hbaoMaterial, passIndex: GetBlurYPassId());
            }

            Blit(cmd, ShaderProperties.inputTex, source, hbaoMaterial, passIndex: GetCompositePassId());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(ShaderProperties.inputTex);
            cmd.ReleaseTemporaryRT(ShaderProperties.hbaoTex);
            cmd.ReleaseTemporaryRT(ShaderProperties.tempTex);
        }

        private void UpdateMaterialProperties(RenderingData renderingData)
        {
            var width = renderingData.cameraData.cameraTargetDescriptor.width;
            var height = renderingData.cameraData.cameraTargetDescriptor.height;

            float tanHalfFovY = Mathf.Tan(0.5f * renderingData.cameraData.camera.fieldOfView * Mathf.Deg2Rad);
            float invFocalLenX = 1.0f / (1.0f / tanHalfFovY * (height / (float)width));
            float invFocalLenY = 1.0f / (1.0f / tanHalfFovY);
            float maxRadInPixels = Mathf.Max(16, settings.maxRadiusPixels * Mathf.Sqrt((width * height) / (1080.0f * 1920.0f)));

            hbaoMaterial.SetVector(ShaderProperties.uvToView, new Vector4(2.0f * invFocalLenX, -2.0f * invFocalLenY, -1.0f * invFocalLenX, 1.0f * invFocalLenY));

            if (settings.resolution != Resolution.Full && (settings.perPixelNormals == PerPixelNormals.Reconstruct4Samples || settings.perPixelNormals == PerPixelNormals.Reconstruct2Samples))
                hbaoMaterial.SetVector(ShaderProperties.targetScale, new Vector4((width + 0.5f) / width, (height + 0.5f) / height, 1f, 1f));
            else
                hbaoMaterial.SetVector(ShaderProperties.targetScale, new Vector4(1f, 1f, 1f, 1f));

            if (m_NoiseTex == null || m_Quality != settings.quality)
            {
                if (m_NoiseTex != null)
                    CoreUtils.Destroy(m_NoiseTex);

                CreateNoiseTexture();
            }

            m_Quality = settings.quality;

            hbaoMaterial.SetVector(ShaderProperties.screenSize, new Vector4(width, height, 1f / width, 1f / height));
            hbaoMaterial.SetTexture(ShaderProperties.noiseTex, m_NoiseTex);
            hbaoMaterial.SetFloat(ShaderProperties.radius, settings.radius * 0.5f * (height / (tanHalfFovY * 2.0f)));
            hbaoMaterial.SetFloat(ShaderProperties.maxRadiusPixels, maxRadInPixels);
            hbaoMaterial.SetFloat(ShaderProperties.negInvRadius2, -1.0f / (settings.radius * settings.radius));
            hbaoMaterial.SetFloat(ShaderProperties.angleBias, settings.bias);
            hbaoMaterial.SetFloat(ShaderProperties.aoMultiplier, 2.0f * (1.0f / (1.0f - settings.bias)));
            hbaoMaterial.SetFloat(ShaderProperties.intensity, settings.intensity);
            hbaoMaterial.SetFloat(ShaderProperties.multiBounceInfluence, settings.multiBounceInfluence);
            hbaoMaterial.SetFloat(ShaderProperties.offscreenSamplesContrib, settings.offscreenSamplesContribution);
            hbaoMaterial.SetFloat(ShaderProperties.maxDistance, settings.maxDistance);
            hbaoMaterial.SetFloat(ShaderProperties.distanceFalloff, settings.distanceFalloff);
            hbaoMaterial.SetColor(ShaderProperties.baseColor, settings.baseColor);
            hbaoMaterial.SetFloat(ShaderProperties.blurSharpness, settings.sharpness);
            hbaoMaterial.SetFloat(ShaderProperties.colorBleedSaturation, settings.saturation);
            hbaoMaterial.SetFloat(ShaderProperties.colorBleedBrightnessMask, settings.brightnessMask);
            hbaoMaterial.SetVector(ShaderProperties.colorBleedBrightnessMaskRange, settings.brightnessMaskRange);

            CoreUtils.SetKeyword(hbaoMaterial, "ORTHOGRAPHIC_PROJECTION", renderingData.cameraData.camera.orthographic);
            CoreUtils.SetKeyword(hbaoMaterial, "OFFSCREEN_SAMPLES_CONTRIB", settings.offscreenSamplesContribution > 0);
            CoreUtils.SetKeyword(hbaoMaterial, "COLOR_BLEEDING", settings.enableColorBleeding);
            CoreUtils.SetKeyword(hbaoMaterial, "NORMALS_RECONSTRUCT2", settings.perPixelNormals == PerPixelNormals.Reconstruct2Samples);
            CoreUtils.SetKeyword(hbaoMaterial, "NORMALS_RECONSTRUCT4", settings.perPixelNormals == PerPixelNormals.Reconstruct4Samples);
        }

        private int GetAoPassId()
        {
            switch (settings.quality)
            {
                case Quality.Lowest:
                    return Pass.AO_LowestQuality;
                case Quality.Low:
                    return Pass.AO_LowQuality;
                case Quality.Medium:
                    return Pass.AO_MediumQuality;
                case Quality.High:
                    return Pass.AO_HighQuality;
                case Quality.Highest:
                    return Pass.AO_HighestQuality;
                default:
                    return Pass.AO_MediumQuality;
            }
        }

        private int GetBlurXPassId()
        {
            switch (settings.blurType)
            {
                case Blur.Narrow:
                    return Pass.Blur_X_Narrow;
                case Blur.Medium:
                    return Pass.Blur_X_Medium;
                case Blur.Wide:
                    return Pass.Blur_X_Wide;
                case Blur.ExtraWide:
                    return Pass.Blur_X_ExtraWide;
                default:
                    return Pass.Blur_X_Medium;
            }
        }

        private int GetBlurYPassId()
        {
            switch (settings.blurType)
            {
                case Blur.Narrow:
                    return Pass.Blur_Y_Narrow;
                case Blur.Medium:
                    return Pass.Blur_Y_Medium;
                case Blur.Wide:
                    return Pass.Blur_Y_Wide;
                case Blur.ExtraWide:
                    return Pass.Blur_Y_ExtraWide;
                default:
                    return Pass.Blur_Y_Medium;
            }
        }

        private int GetCompositePassId()
        {
            switch (settings.displayMode)
            {
                case DisplayMode.Normal:
                    return settings.useMultiBounce ? Pass.Composite_MultiBounce : Pass.Composite;
                case DisplayMode.AOOnly:
                    return Pass.Debug_AO_Only;
                case DisplayMode.ColorBleedingOnly:
                    return Pass.Debug_ColorBleeding_Only;
                case DisplayMode.SplitWithoutAOAndWithAO:
                    return Pass.Debug_Split_WithoutAO_WithAO;
                case DisplayMode.SplitWithAOAndAOOnly:
                    return Pass.Debug_Split_WithAO_AOOnly;
                case DisplayMode.SplitWithoutAOAndAOOnly:
                    return Pass.Debug_Split_WithoutAO_AOOnly;
                case DisplayMode.ViewNormals:
                    return Pass.Debug_ViewNormals;
                default:
                    return Pass.Composite;
            }
        }

        private void CreateNoiseTexture()
        {
            m_NoiseTex = new Texture2D(4, 4, TextureFormat.RGB24, false, true);
            m_NoiseTex.filterMode = FilterMode.Point;
            m_NoiseTex.wrapMode = TextureWrapMode.Repeat;
            int z = 0;
            for (int x = 0; x < 4; ++x)
            {
                for (int y = 0; y < 4; ++y)
                {
                    float r1 = MersenneTwister.Numbers[z++];
                    float r2 = MersenneTwister.Numbers[z++];
                    float angle = 2.0f * Mathf.PI * r1 / m_NumSampleDirections[GetAoPassId()];
                    Color color = new Color(Mathf.Cos(angle), Mathf.Sin(angle), r2);
                    m_NoiseTex.SetPixel(x, y, color);
                }
            }
            m_NoiseTex.Apply();
        }
    }

    public HBAORendererSettings settings;
    public Shader hbaoShader;

    private HBAORenderPass m_HBAORenderPass;
    private const string SHADER_PATH = "Hidden/Universal Render Pipeline/HBAO";

    public override void Create()
    {
        m_HBAORenderPass = new HBAORenderPass();
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings == null)
        {
            Debug.LogWarning("Please attach HBAORendererSettings to settings field");
            return;
        }

        if (hbaoShader == null)
        {
            hbaoShader = Shader.Find(SHADER_PATH);
            if (hbaoShader == null)
            {
                Debug.LogWarning("HBAO shader was not found. Please ensure it compiles correctly");
                return;
            }
        }

        if (!settings.enable) return;

        // Configures where the render pass should be injected.
        m_HBAORenderPass.renderPassEvent = settings.displayMode == DisplayMode.Normal ?
            RenderPassEvent.BeforeRenderingTransparents :
            RenderPassEvent.AfterRenderingTransparents;

        m_HBAORenderPass.settings = settings;

        var material = CoreUtils.CreateEngineMaterial(hbaoShader);

        m_HBAORenderPass.Setup(material, renderer.cameraColorTarget);

        renderer.EnqueuePass(m_HBAORenderPass);
    }
}


