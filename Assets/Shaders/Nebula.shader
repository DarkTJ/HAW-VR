Shader "Unlit/NebulaConvert"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			#define MOD3 float3(.1031,.11369,.13787)

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}



			float hash11(float p) {
				// From Dave Hoskins
				float3 p3 = frac(float3(p, p, p) * MOD3);
				p3 += dot(p3, p3.yzx + 19.19);
				return frac((p3.x + p3.y) * p3.z);
			}

			//  1 out, 2 in...
			float hash12(float2 p) {
				float3 p3 = frac(float3(p.xyx) * MOD3);
				p3 += dot(p3, p3.yzx + 19.19);
				return frac((p3.x + p3.y) * p3.z);
			}

			//https://www.shadertoy.com/view/4tlSzl for Voronoi 

			float3 hash33(float3 p) {
				float n = sin(dot(p, float3(7, 157, 113)));
				return frac(float3(2097152, 262144, 32768) * n);
			}

			float voronoi(float3 p) {
				float3 b, r, g = floor(p);
				p = frac(p);
				float d = 1.;
				for (int j = -1; j <= 1; j++) {
					for (int i = -1; i <= 1; i++) {
						b = float3(i, j, -1);
						r = b - p + hash33(g + b);
						d = min(d, dot(r, r));
						b.z = 0.0;
						r = b - p + hash33(g + b);
						d = min(d, dot(r, r));
						b.z = 1.;
						r = b - p + hash33(g + b);
						d = min(d, dot(r, r));
					}
				}

				return d; // Range: [0, 1]
			}

			float nebula(in float2 uv) {
				float offset = 1.;
				float size = .6;
				float map = 0.;
				float t;
				for (int i = 1; i <= 3; i++) {
					t = _Time.y * .05;
					size *= 2.5;
					map += voronoi(float3(uv * size, 0.6) + float3(t, t, t)) * offset;
					offset *= .25;
				}
				return pow(map, 1.0);
			}

			float star(float2 uv, float time) {
				float2 twinkle = frac(uv * time) - 0.2;
				float2 n = floor(uv * time);
				twinkle += float2(sin(hash12(n) * 200.23) * .3, sin(hash12(n) * 914.19) * .3);
				float map = .1 * abs(sin(time * 250.12)) * .5 / length(twinkle) * .025;
				return map;
			}

			float stars(in float2 uv, float time) {
				float size = 5.0;
				float map = 0.;
				for (int i = 1; i <= 7; i++) {
					float3 t = float3(0., 0., time + _Time.y * .3);
					size *= 1.2;
					map += star(uv * size + t, 1. + t);
				}

				return map;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float3 col = .0;

				float3 col1 = normalize(float3(223.3, 100., 37.3)) * nebula(uv);
				float3 col2 = normalize(float3(170.7, 80, 85.7)) * nebula(uv + float2(0., 0.05 * _Time.y));
				col2 = pow(col2, 1.2);
				col += lerp(col1, col2, 2.5);
				float m = stars(uv, _Time.y * 2.);
				col += float3(m, m, m);

				return float4(col, 1.0);
			}



			ENDCG
		}
	}
}
