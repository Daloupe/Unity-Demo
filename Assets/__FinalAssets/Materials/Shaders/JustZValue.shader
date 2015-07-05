﻿// Copyright (C) Stanislaw Adaszewski, 2013
// http://algoholic.eu

Shader "Custom/JustZValue" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"RenderType"="Opaque" "Queue"="Background-3" }
		LOD 500

		ColorMask 0
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		//void alph(Input In, inout )
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}