Shader "Hidden/Custom/Print"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_Watermark, sampler_Watermark);

	float3 _InkColour;
	float4 _PaperColour;
	float _PrintBlackLevel;
	float _WatermarkDepth;
	float4x4 _WatermarkMatrix;
	float2 _WatermarkPadding;
	float _WatermarkSkew;

	// extracted from shader graph
	float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
	{
		float2 i = floor(uv);
		float2 f = frac(uv);
		f = f * f * (3.0 - 2.0 * f);
		uv = abs(frac(uv) - 0.5);
		float2 c0 = i + float2(0.0, 0.0);
		float2 c1 = i + float2(1.0, 0.0);
		float2 c2 = i + float2(0.0, 1.0);
		float2 c3 = i + float2(1.0, 1.0);
		float r0; Hash_Tchou_2_1_float(c0, r0);
		float r1; Hash_Tchou_2_1_float(c1, r1);
		float r2; Hash_Tchou_2_1_float(c2, r2);
		float r3; Hash_Tchou_2_1_float(c3, r3);
		float bottomOfGrid = lerp(r0, r1, f.x);
		float topOfGrid = lerp(r2, r3, f.x);
		float t = lerp(bottomOfGrid, topOfGrid, f.y);
		return t;
	}
	
	float4 frag (VaryingsDefault i) : SV_Target
	{
		float2 uv = i.texcoord * float2(3840.0f, 2160.0f);
		float3 output = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
		float2 uv2 = i.texcoord;
		uv2 = mul((float4x4)_WatermarkMatrix, float4(uv2, 0.0f, 1.0f));
		float n = trunc(uv2.y * _WatermarkPadding.y);
		uv2.x = uv2.x + n * _WatermarkSkew;
		uv2 = frac(uv2) * _WatermarkPadding;
		float watermark = 1.0f - SAMPLE_TEXTURE2D(_Watermark, sampler_Watermark, uv2).a * _WatermarkDepth;
		float3 source = output.rgb;
		float3 paper = _PaperColour.rgb * Unity_SimpleNoise_ValueNoise_Deterministic_float(uv) * watermark;
		float l = Luminance(source);
		return float4(lerp(_InkColour, paper, step(_PrintBlackLevel, l)), l * _PaperColour.a);
	}

	ENDHLSL
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment frag
			ENDHLSL
		}
	}
}
