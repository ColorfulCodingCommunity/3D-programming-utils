Shader "Unlit/CutoutShaderWithGradient"
{
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_CutoutTex("Texture", 2D) = "white" {}
		_Cutout("Alpha Cutout", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 100

		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				sampler2D _CutoutTex;
				float4 _MainTex_ST;
				fixed _Cutout;

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					float val = tex2D(_CutoutTex, i.texcoord).r;

					fixed cutoff = 0.4;
					clip(col.a - cutoff);
					
					float edgeHeight = 0.015;
					col = lerp(
						col,
						fixed4(0, 4, 2, 1),
						step(col.a, cutoff + edgeHeight)
					);

					fixed gradient = 0.05;
					if(_Cutout != 0 && val <= _Cutout) 
						col.a = 0;
					if(_Cutout != 1 && val<=_Cutout && _Cutout - val < gradient) 
						col.a = (1 / gradient) * (gradient - (_Cutout - val));

					return col;

				}
			ENDCG
		}
	}
}