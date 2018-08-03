Shader "Custom/TexturesOverlay" {
	Properties {
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_SecTex("Color (RGB) Alpha (A)", 2D) = "white" {}
		_TerTex("Color (RGB) Alpha (A)", 2D) = "white" {}

		_MainOpacity ("Main Opacity", Range(0,1)) = 1
		_SecOpacity ("Secondary Opacity", Range(0,1)) = 1
		_TerOpacity("Tertiary Opacity", Range(0,1)) = 1
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecTex;
		sampler2D _TerTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SecTex;
			float2 uv_TerTex;
		};

		half _MainOpacity;
		half _SecOpacity;
		half _TerOpacity;

		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_SecTex, IN.uv_SecTex);
			fixed4 c3 = tex2D(_TerTex, IN.uv_TerTex);


			c  *= _MainOpacity;
			c2 *= _SecOpacity;
			c3 *= _TerOpacity;


			if (c2.a > 0) {
				if (c.a >= 0) {
					c = c*(1 - _SecOpacity) + c2;
				}
				else {
					c = c2;
				}
			}

			if (c3.a > 0) {
				if (c.a >= 0) {
					c = c*(1 - _TerOpacity) + c3;
				}
				else {
					c = c3;
				}
			}



			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
