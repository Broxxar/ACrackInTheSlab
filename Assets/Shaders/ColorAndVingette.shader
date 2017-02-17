Shader "Hidden/ColorAndVingette"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
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
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			half _MinRadius;
			half _MaxRadius;
			half _Saturation;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed lum = col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
				col = lerp(lum, col, _Saturation);

				half aspect = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
				half2 uv = (i.uv * 2 - 1);

				half dist = length(uv / aspect);

				half vingette = smoothstep(_MinRadius, _MaxRadius, dist);

				return col * (1 - vingette);
			}
			ENDCG
		}
	}
}
