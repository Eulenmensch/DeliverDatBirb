//----------------------------------------------------------------------------------
//
// Copyright (c) 2014, NVIDIA CORPORATION. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//  * Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//  * Neither the name of NVIDIA CORPORATION nor the names of its
//    contributors may be used to endorse or promote products derived
//    from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ``AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//----------------------------------------------------------------------------------

#ifndef HBAO_AO_INCLUDED
#define HBAO_AO_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "HBAO_Common.hlsl"

inline float3 FetchViewPos(float2 uv) {
    float depth = LinearizeDepth(FetchRawDepth(uv));
    return float3((uv * _UVToView.xy + _UVToView.zw) * depth, depth);
}

inline float Falloff(float distanceSquare) {
    // 1 scalar mad instruction
    return distanceSquare * _NegInvRadius2 + 1.0;
}

inline float ComputeAO(float3 P, float3 N, float3 S) {
    float3 V = S - P;
    float VdotV = dot(V, V);
    float NdotV = dot(N, V) * rsqrt(VdotV);

    // Use saturate(x) instead of max(x,0.f) because that is faster on Kepler
    return saturate(NdotV - _AngleBias) * saturate(Falloff(VdotV));
}

inline float3 MinDiff(float3 P, float3 Pr, float3 Pl) {
    float3 V1 = Pr - P;
    float3 V2 = P - Pl;
    return (dot(V1, V1) < dot(V2, V2)) ? V1 : V2;
}

inline float2 RotateDirections(float2 dir, float2 rot) {
    return float2(dir.x * rot.x - dir.y * rot.y,
        dir.x * rot.y + dir.y * rot.x);
}

float4 AO_Frag(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);
    //uint2 positionSS = input.uv * _ScreenSize.xy;

    float3 P = FetchViewPos(uv);

    clip(_MaxDistance - P.z);

    float stepSize = min((_Radius / P.z), _MaxRadiusPixels) / (STEPS + 1.0);

    // (cos(alpha), sin(alpha), jitter)
    //float3 rand = SAMPLE_TEXTURE2D(_NoiseTex, sampler_PointRepeat, positionSS / 4.0).rgb;
    float3 rand = SAMPLE_TEXTURE2D(_NoiseTex, sampler_PointRepeat, input.positionCS.xy / 4.0).rgb;

#if NORMALS_RECONSTRUCT4
    float3 Pr, Pl, Pt, Pb;
    Pr = FetchViewPos(uv + float2(_ScreenSize.z, 0));
    Pl = FetchViewPos(uv + float2(-_ScreenSize.z, 0));
    Pt = FetchViewPos(uv + float2(0, _ScreenSize.w));
    Pb = FetchViewPos(uv + float2(0, -_ScreenSize.w));
    float3 N = normalize(cross(MinDiff(P, Pr, Pl), MinDiff(P, Pt, Pb)));
#else // NORMALS_RECONSTRUCT2
    float3 Pr, Pt;
    Pr = FetchViewPos(uv + float2(_ScreenSize.z, 0));
    Pt = FetchViewPos(uv + float2(0, _ScreenSize.w));
    float3 N = normalize(cross(Pt - P, P - Pr));
#endif

    const float alpha = 2.0 * PI / DIRECTIONS;
    float ao = 0;
    half3 col = half3(0, 0, 0);

    UNITY_UNROLL
    for (int d = 0; d < DIRECTIONS; ++d) {
        float angle = alpha * float(d);

        // Compute normalized 2D direction
        float cosA, sinA;
        sincos(angle, sinA, cosA);
        float2 direction = RotateDirections(float2(cosA, sinA), rand.xy);

        // Jitter starting sample within the first step
        float rayPixels = (rand.z * stepSize + 1.0);

        UNITY_UNROLL
        for (int s = 0; s < STEPS; ++s) {

            float2 snappedUV = round(rayPixels * direction) * _ScreenSize.zw + uv;
            float3 S = FetchViewPos(snappedUV);

            rayPixels += stepSize;

            float contrib = ComputeAO(P, N, S);
#if OFFSCREEN_SAMPLES_CONTRIB
            float2 offscreenAmount = _OffscreenSamplesContrib * (snappedUV - saturate(snappedUV) != 0 ? 1 : 0);
            contrib = max(contrib, offscreenAmount.x);
            contrib = max(contrib, offscreenAmount.y);
#endif
            ao += contrib;
#if COLOR_BLEEDING
            half3 emission = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, snappedUV, 0).rgb;
            half average = (emission.x + emission.y + emission.z) / 3;
            half scaledAverage = saturate((average - _ColorBleedBrightnessMaskRange.x) / (_ColorBleedBrightnessMaskRange.y - _ColorBleedBrightnessMaskRange.x + 1e-6));
            half maskMultiplier = 1 - (scaledAverage * _ColorBleedBrightnessMask);
            col += emission * contrib * maskMultiplier;
#endif
        }
    }

#if defined(DEBUG_VIEWNORMALS)
    N = float3(N.x, -N.y, N.z);
    return float4(N * 0.5 + 0.5, 1);
#else
#if COLOR_BLEEDING
    float4 aoOutput = float4(col, ao);
#else
    float aoOutput = ao;
#endif

    // apply bias multiplier
    aoOutput *= (_AOmultiplier / (STEPS * DIRECTIONS));

    float fallOffStart = _MaxDistance - _DistanceFalloff;
    float distFactor = saturate((P.z - fallOffStart) / (_MaxDistance - fallOffStart));
#if COLOR_BLEEDING
    //aoOutput.rgb = pow(abs(aoOutput.rgb), 1 / _ColorBleedSaturation);
    aoOutput.rgb = saturate(1 - lerp(dot(aoOutput.rgb, 0.333).xxx, aoOutput.rgb, _ColorBleedSaturation));
    aoOutput = lerp(saturate(float4(aoOutput.rgb, 1 - aoOutput.a)), float4(1, 1, 1, 1), distFactor);
    return aoOutput;
#else
    aoOutput = lerp(saturate(1 - aoOutput), 1, distFactor);
    return float4(EncodeFloatRGB(saturate(P.z * (1.0 / _ProjectionParams.z))), aoOutput);
#endif
#endif
}

#endif // HBAO_AO_INCLUDED
