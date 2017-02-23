Shader "Cut Image Effect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OddTex ("Odd", 2D) = "white" {}
		_EvenTex ("Even", 2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 o_uv : TEXCOORD1;
				float2 e_uv : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _OddTex;
			sampler2D _EvenTex;
			sampler2D _FillPrepassTex;
			float4 _OddTex_ST;
			float4 _EvenTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				o.o_uv = TRANSFORM_TEX(v.uv, _OddTex);
				o.e_uv = TRANSFORM_TEX(v.uv, _EvenTex);

				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				i.uv.y = 1-i.uv.y;
				fixed4 col = tex2D(_FillPrepassTex, i.uv);
				
				fixed4 outcol = 0;

				float2 even_uv = (i.e_uv * _ScreenParams.xy)/100.0;
				outcol += (col) * tex2D(_EvenTex, even_uv);

				float2 odd_uv = (i.o_uv * _ScreenParams.xy)/100.0;
				outcol += (1-col) * tex2D(_OddTex, odd_uv);

				return outcol;
			}
			ENDCG
		}
	}
}
