Shader "Special/TimePiece"
{
	Properties
	{
		_BumpMap("Normal", 2D) = "normal" {}
		_Refraction("Refraction Magnitude", Range(-0.02, 0.02)) = 0.015
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct FragIn
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 screen_uv : TEXCOORD1;
				float3 tangentToWorld[3] : TEXCOORD2;//TEXCOORD3;TEXCOORD4;
			};
			
			FragIn vert (VertIn v)
			{
				FragIn o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screen_uv = float3((o.vertex.xy + o.vertex.w) * 0.5, o.vertex.w);
				//jack up uvs
				//o.screen_uv = float3((o.vertex.xy / o.vertex.w + 1) * 0.5, o.vertex.w);

				// Normal Mapping Stuff
				o.normal = UnityObjectToWorldNormal(v.normal);
				float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
				float3x3 tangentToWorld = CreateTangentToWorldPerVertex(o.normal, tangentWorld.xyz, tangentWorld.w);
				o.tangentToWorld[0].xyz = tangentToWorld[0];
				o.tangentToWorld[1].xyz = tangentToWorld[1];
				o.tangentToWorld[2].xyz = tangentToWorld[2];

				return o;
			}

			sampler2D _BumpMap;
			sampler2D _TimeCrackTexture;
			float _Refraction;

			fixed4 frag (FragIn i) : SV_Target
			{
				// Normal Mapping Stuff
				float3 tangent = i.tangentToWorld[0].xyz;
				float3 binormal = i.tangentToWorld[1].xyz;
				float3 normal = i.tangentToWorld[2].xyz;
				float3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv));
				float3 normalWorld = normalize(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z);

				// Refraction Vector from world Normal
				float3 viewSpaceNormal = mul(UNITY_MATRIX_V, normalWorld);
				float2 refractionVector = viewSpaceNormal.xy * viewSpaceNormal.z  * _Refraction;

				// Perspective correction for screen uv coordinate
				float2 screen_uv = i.screen_uv.xy / i.screen_uv.z;

				// Do the thing!
				fixed4 col = tex2D(_TimeCrackTexture, screen_uv + refractionVector);

				return col;
			}
			ENDCG
		}
	}
}
