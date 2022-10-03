Shader "Custom/Phosph" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {} // Regular object texture 
	}
	SubShader{
		ZWrite Off
        ZTest Always

		// draw after all opaque objects (queue = 2001):
		Tags { "Queue" = "Overlay+1" }
		Pass {

		}
	}
}