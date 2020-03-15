#ifndef HBAO_COMMON_INCLUDED
#define HBAO_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

inline float FetchRawDepth(float2 uv) {
    return SampleSceneDepth(uv * _TargetScale.xy);
}

inline float LinearizeDepth(float depth) {
    // References: https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
#if ORTHOGRAPHIC_PROJECTION
#if UNITY_REVERSED_Z
    depth = 1 - depth;
#endif // UNITY_REVERSED_Z
    float linearDepth = _ProjectionParams.y + depth * (_ProjectionParams.z - _ProjectionParams.y); // near + depth * (far - near)
#else
    float linearDepth = LinearEyeDepth(depth, _ZBufferParams);
#endif // ORTHOGRAPHIC_PROJECTION
    return linearDepth;
}

inline float3 EncodeFloatRGB(float value) {
    const float max24int = 256 * 256 * 256 - 1;
    float3 encoded = floor(value * float3(max24int / (256 * 256), max24int / 256, max24int)) / 255.0;
    encoded.z -= encoded.y * 256.0;
    encoded.y -= encoded.x * 256.0;
    return encoded;
}

inline float DecodeFloatRGB(float3 value) {
    return dot(value, float3(255.0 / 256, 255.0 / (256 * 256), 255.0 / (256 * 256 * 256)));
}

#endif // HBAO_COMMON_INCLUDED
