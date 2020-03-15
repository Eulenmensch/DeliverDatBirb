#ifndef HBAO_REINTERLEAVE_FRAG_INCLUDED
#define HBAO_REINTERLEAVE_FRAG_INCLUDED

	half4 frag(v2f i) : SV_Target {
		float2 offset = fmod(floor(i.uv2 * _FullRes_TexelSize.zw), DOWNSCALING_FACTOR);
		float2 uv = (floor(i.uv2 * _LayerRes_TexelSize.zw) + (offset * _LayerRes_TexelSize.zw) + 0.5) * _FullRes_TexelSize.xy;
		return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv);
	}

#endif // HBAO_REINTERLEAVE_FRAG_INCLUDED
