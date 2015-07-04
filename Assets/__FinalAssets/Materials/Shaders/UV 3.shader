﻿Shader "Debug/UV 3" {
SubShader {
	Pass {
		Fog { Mode Off }
		Tags { "LightMode" = "ForwardBase" }
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		#pragma multi_compile_fwdbase
		#include "AutoLight.cginc"


		// vertex input: position, UV
		struct appdata {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			LIGHTING_COORDS(1,2)
		};
		
		v2f vert (appdata v) {
			v2f o;
			o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
			o.uv = float4( v.texcoord.xy, 0.5, 0.0);
			TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}
		
		half4 frag( v2f i ) : SV_Target {
			half4 c = frac( i.uv );
			if (any( i.uv - saturate(i.uv)))
				c.b = 0.75;

			float attenuation = LIGHT_ATTENUATION(i);

			return c * attenuation;
		}
		ENDCG

		
	}

}
//Fallback "VertexLit"
}