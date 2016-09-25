Shader "GlowOutline"
{
	Properties
	{
		[HideInInspector] _MainTex("Albedo", 2D) = "white" {}

		_GlowColor("Glow Color", Color) = (1,1,1,1)

		[HideInInspector] _OccludeMap("Occlude Map", 2D) = "white" {}

			// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
		LOD 300
			
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OccludeMap;

			half4 frag(v2f_img IN) : COLOR{
				return tex2D(_MainTex, IN.uv) - tex2D(_OccludeMap, IN.uv);
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OccludeMap;

			half4 frag(v2f_img IN) : COLOR{
				return tex2D(_MainTex, IN.uv) + tex2D(_OccludeMap, IN.uv);
			}
			ENDCG
		}
				Pass{
				Tags{ "RenderType" = "Opaque" }
				ZWrite On ZTest Always Fog{ Mode Off }

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

				fixed4 _GlowColor;
			float4 vert(float4 v:POSITION) : POSITION{
				return mul(UNITY_MATRIX_MVP, v);
			}
				fixed4 frag() : SV_Target{
				return fixed4(_GlowColor.rgb, 1.0);
			}
				ENDCG
			}
	}
}