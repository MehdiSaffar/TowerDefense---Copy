Shader "Unlit/HealthBar"
{
	Properties
	{
		_WaveSpeed ("Wave Speed", Range(1, 10)) = 1
		_FirstColor ("First Color", Color) = (1,1,1,1)
		_SecondColor ("Second Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Name "Wavy"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _FirstColor;
			float4 _SecondColor;
			float _WaveSpeed;
			
			float4 vert(appdata_base v) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, v.vertex);
			}
			
			float4 frag (float4 pos : SV_POSITION) : SV_Target
			{
				return lerp(_FirstColor, _SecondColor, sin(_Time.w * _WaveSpeed));
			}
			ENDCG
		}
	}
}
