using UnityEngine;

[CreateAssetMenu(fileName = "HBAORendererSettings", menuName = "HBAO/HBAO Renderer Settings", order = 100)]
public class HBAORendererSettings : ScriptableObject
{
    public bool enable = true;

    [Header("General")]
    [Tooltip("The quality of the AO.")]
    public HBAORendererFeature.Quality quality = HBAORendererFeature.Quality.Medium;
    [Tooltip("The resolution at which the AO is calculated.")]
    public HBAORendererFeature.Resolution resolution = HBAORendererFeature.Resolution.Full;
    [Tooltip("The way the AO is displayed on screen.")]
    public HBAORendererFeature.DisplayMode displayMode = HBAORendererFeature.DisplayMode.Normal;

    [Header("AO Settings")]
    [Tooltip("AO radius: this is the distance outside which occluders are ignored.")]
    [Range(0.25f, 5)]
    public float radius = 1f;
    [Tooltip("Maximum radius in pixels: this prevents the radius to grow too much with close-up " +
             "object and impact on performances.")]
    [Range(16, 256)]
    public float maxRadiusPixels = 128f;
    [Tooltip("For low-tessellated geometry, occlusion variations tend to appear at creases and " +
             "ridges, which betray the underlying tessellation. To remove these artifacts, we use " +
             "an angle bias parameter which restricts the hemisphere.")]
    [Range(0, 0.5f)]
    public float bias = 0.2f;
    [Tooltip("This value allows to scale up the ambient occlusion values.")]
    [Range(0, 4)]
    public float intensity = 1f;
    [Tooltip("Enable/disable MultiBounce approximation.")]
    public bool useMultiBounce = false;
    [Tooltip("MultiBounce approximation influence.")]
    [Range(0, 1)]
    public float multiBounceInfluence = 1f;
    [Tooltip("The amount of AO offscreen samples are contributing.")]
    [Range(0, 1)]
    public float offscreenSamplesContribution = 0f;
    [Tooltip("The max distance to display AO.")]
    public float maxDistance = 150f;
    [Tooltip("The distance before max distance at which AO start to decrease.")]
    public float distanceFalloff = 50f;
    [Tooltip("The type of per pixel normals to use.")]
    public HBAORendererFeature.PerPixelNormals perPixelNormals = HBAORendererFeature.PerPixelNormals.Reconstruct4Samples;
    [Tooltip("This setting allow you to set the base color if the AO, the alpha channel value is unused.")]
    public Color baseColor = Color.black;

    [Header("Blur Settings")]
    [Tooltip("The type of blur to use.")]
    public HBAORendererFeature.Blur blurType = HBAORendererFeature.Blur.Medium;
    [Tooltip("This parameter controls the depth-dependent weight of the bilateral filter, to " +
             "avoid bleeding across edges. A zero sharpness is a pure Gaussian blur. Increasing " +
             "the blur sharpness removes bleeding by using lower weights for samples with large " +
             "depth delta from the current pixel.")]
    [Range(0, 16)]
    public float sharpness = 8f;
    //[Tooltip("Is the blur downsampled.")]
    //public bool blurDownsample = false;

    [Header("Color Bleeding Settings")]
    public bool enableColorBleeding = false;
    [Tooltip("This value allows to control the saturation of the color bleeding.")]
    [Range(0, 2)]
    public float saturation = 0.67f;
    [Tooltip("Use masking on emissive pixels")]
    [Range(0, 1)]
    public float brightnessMask = 1f;
    [Tooltip("Brightness level where masking starts/ends")]
    [HBAO_MinMaxSlider(0, 8)]
    public Vector2 brightnessMaskRange = new Vector2(0.0f, 1.0f);
}