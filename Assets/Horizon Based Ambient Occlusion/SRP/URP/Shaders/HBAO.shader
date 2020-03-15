Shader "Hidden/Universal Render Pipeline/HBAO"
{
    Properties
    {
        _MainTex("", any) = "" {}
        _HBAOTex("", any) = "" {}
        _NoiseTex("", 2D) = "" {}
    }

    HLSLINCLUDE

    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    //#pragma enable_d3d11_debug_symbols

    #pragma multi_compile_local __ ORTHOGRAPHIC_PROJECTION
    #pragma multi_compile_local __ COLOR_BLEEDING
    #pragma multi_compile_local __ OFFSCREEN_SAMPLES_CONTRIB
    //#pragma multi_compile_local NORMALS_CAMERA NORMALS_RECONSTRUCT2 NORMALS_RECONSTRUCT4
    #pragma multi_compile_local NORMALS_RECONSTRUCT2 NORMALS_RECONSTRUCT4

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    struct Attributes
    {
        float4 positionOS   : POSITION;
        float2 uv           : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS    : SV_POSITION;
        float2 uv            : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        return output;
    }

    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X(_HBAOTex);
    TEXTURE2D(_NoiseTex);
    SAMPLER(sampler_LinearClamp);
    SAMPLER(sampler_PointRepeat);
    SAMPLER(sampler_PointClamp);

    CBUFFER_START(FrequentlyUpdatedUniforms)
    float4 _UVToView;
    float _Radius;
    float _MaxRadiusPixels;
    float _NegInvRadius2;
    float _AngleBias;
    float _AOmultiplier;
    float _Intensity;
    half4 _BaseColor;
    float _NoiseTexSize;
    float _MultiBounceInfluence;
    float _OffscreenSamplesContrib;
    float _MaxDistance;
    float _DistanceFalloff;
    float _BlurSharpness;
    float _ColorBleedSaturation;
    float _ColorBleedBrightnessMask;
    float2 _ColorBleedBrightnessMaskRange;
    float4 _TargetScale;
    CBUFFER_END
  
    ENDHLSL

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        ZWrite Off ZTest Always Blend Off Cull Off

        Pass // 0
        {
            Name "HBAO - AO Lowest Quality"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		3
            #define STEPS			2
            #include "HBAO_AO.hlsl"
            ENDHLSL
        }

        Pass // 1
        {
            Name "HBAO - AO Low Quality"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		4
            #define STEPS			3
            #include "HBAO_AO.hlsl"
            ENDHLSL
        }

        Pass // 2
        {
            Name "HBAO - AO Medium Quality"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		6
            #define STEPS			4
            #include "HBAO_AO.hlsl"
            ENDHLSL
        }

        Pass // 3
        {
            Name "HBAO - AO High Quality"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		8
            #define STEPS			4
            #include "HBAO_AO.hlsl"
            ENDHLSL
        }

        Pass // 4
        {
            Name "HBAO - AO Highest Quality"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		8
            #define STEPS			6
            #include "HBAO_AO.hlsl"
            ENDHLSL
        }

        Pass // 5
        {
            Name "HBAO - Blur X Narrow"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_X_Frag

            #define KERNEL_RADIUS		2
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 6
        {
            Name "HBAO - Blur X Medium"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_X_Frag

            #define KERNEL_RADIUS		3
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 7
        {
            Name "HBAO - Blur X Wide"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_X_Frag

            #define KERNEL_RADIUS		4
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 8
        {
            Name "HBAO - Blur X ExtraWide"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_X_Frag

            #define KERNEL_RADIUS		5
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 9
        {
            Name "HBAO - Blur Y Narrow"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_Y_Frag

            #define KERNEL_RADIUS		2
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 10
        {
            Name "HBAO - Blur Y Medium"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_Y_Frag

            #define KERNEL_RADIUS		3
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 11
        {
            Name "HBAO - Blur Y Wide"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_Y_Frag

            #define KERNEL_RADIUS		4
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 12
        {
            Name "HBAO - Blur Y ExtraWide"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Blur_Y_Frag

            #define KERNEL_RADIUS		5
            #include "HBAO_Blur.hlsl"
            ENDHLSL
        }

        Pass // 13
        {
            Name "HBAO - Composite"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 14
        {
            Name "HBAO - Composite with multibounce"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define MULTIBOUNCE

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 15
        {
            Name "HBAO - Debug AO"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define DEBUG_AO

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 16
        {
            Name "HBAO - Debug ColorBleeding"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define DEBUG_COLORBLEEDING

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 17
        {
            Name "HBAO - Debug Without AO / With AO"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define DEBUG_NOAO_AO

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 18
        {
            Name "HBAO - Debug With AO / AO Only"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define DEBUG_AO_AOONLY

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 19
        {
            Name "HBAO - Debug Without AO / AO Only"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Composite_Frag

            #define DEBUG_NOAO_AOONLY

            #include "HBAO_Composite.hlsl"
            ENDHLSL
        }

        Pass // 20
        {
            Name "HBAO - Debug View Normals"

            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment AO_Frag

            #define DIRECTIONS		1
            #define STEPS			1
            #define DEBUG_VIEWNORMALS

            #include "HBAO_AO.hlsl"
            ENDHLSL
        }
    }

    Fallback Off
}
