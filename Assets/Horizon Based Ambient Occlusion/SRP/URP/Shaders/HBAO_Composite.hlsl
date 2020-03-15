#ifndef HBAO_COMPOSITE_INCLUDED
#define HBAO_COMPOSITE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "HBAO_Common.hlsl"

inline float4 FetchOcclusion(float2 uv) {
    return SAMPLE_TEXTURE2D_X(_HBAOTex, sampler_PointClamp, uv);
}

inline half4 FetchSceneColor(float2 uv) {
    //return LOAD_TEXTURE2D_X(_MainTex, positionSS); // load not supported on GLES2
    return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
}

inline half3 MultiBounceAO(float visibility, half3 albedo) {
    half3 a = 2.0404 * albedo - 0.3324;
    half3 b = -4.7951 * albedo + 0.6417;
    half3 c = 2.7552 * albedo + 0.6903;

    float x = visibility;
    return max(x, ((x * a + b) * x + c) * x);
}

float4 Composite_Frag(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    //uint2 positionSS = input.uv * _ScreenSize.xy;

    float4 ao = FetchOcclusion(input.uv);

    ao.a = saturate(pow(abs(ao.a), _Intensity));
    half3 aoColor = lerp(_BaseColor.rgb, half3(1.0, 1.0, 1.0), ao.a);

    half4 col = FetchSceneColor(input.uv);
#if defined(MULTIBOUNCE)
    col.rgb *= lerp(aoColor, MultiBounceAO(ao.a, lerp(col.rgb, _BaseColor.rgb, _BaseColor.rgb)), _MultiBounceInfluence);
#else
    col.rgb *= aoColor;
#endif
#if COLOR_BLEEDING
    col.rgb += 1 - ao.rgb;
#endif

#if defined(DEBUG_AO)
    col = ao.aaaa;
#else
#if defined(DEBUG_COLORBLEEDING) && COLOR_BLEEDING
    col = float4(1 - ao.rgb, 1);
#else
#if defined(DEBUG_NOAO_AO) || defined(DEBUG_AO_AOONLY) || defined(DEBUG_NOAO_AOONLY)
    if (input.uv.x <= 0.4985) {
#if defined(DEBUG_NOAO_AO) || defined(DEBUG_NOAO_AOONLY)
        col = FetchSceneColor(input.uv);
#endif // DEBUG_NOAO_AO || DEBUG_NOAO_AOONLY
        return col;
    }
    if (input.uv.x > 0.4985 && input.uv.x < 0.5015) {
        return float4(0, 0, 0, 1);
    }
#if defined(DEBUG_AO_AOONLY) || defined(DEBUG_NOAO_AOONLY)
    col = ao.aaaa;
#endif // defined(DEBUG_AO_AOONLY) || defined(DEBUG_NOAO_AOONLY)
#endif // defined(DEBUG_NOAO_AO) || defined(DEBUG_AO_AOONLY) || defined(DEBUG_NOAO_AOONLY)
#endif // defined(DEBUG_COLORBLEEDING) && COLOR_BLEEDING
#endif // defined(DEBUG_AO)
    return col;
}

#endif // HBAO_COMPOSITE_INCLUDED
