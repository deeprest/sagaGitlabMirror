Shader "Custom/index-sprite-emissive" {
	Properties {
		_Color ("Multiply Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_BlendColor ("Blend Color", Color) = (1,1,1,1)
		_BlendAmount("Blend Alpha", Range(0.0, 1.0)) = 0

		_IndexColor ("Index 1", Color) = (1,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicTex ("Metallic (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0.0,1.0)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissiveTex ("Emissive (RGB)", 2D) = "white" {}
		_EmissiveAmount ("Amount", Range(0,1)) = 0.0

	}
	SubShader {
		Tags { 
		"Queue"="Transparent" 
		"IgnoreProjector"="True" 
		"RenderType"="Transparent" 
		"PreviewType"="Plane"
		}
		LOD 200
		Cull Off
		Lighting On
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicTex;
		sampler2D _EmissiveTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _EmissiveAmount;
		fixed4 _Color;
		fixed4 _IndexColor;

		fixed4 _BlendColor;
		half _BlendAmount;
        

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;

			int r = (int)(c.r * 255.0);
			if( r==1 )
				o.Albedo = lerp( _IndexColor.rgb * min(1.0,c.g*2.0), float4(1,1,1,1), (c.g-0.5)*2.0 );


			o.Albedo = lerp( o.Albedo, _BlendColor, _BlendAmount );
            o.Normal = float3(0,0,1);
			o.Metallic = _Metallic * (1.0 - tex2D( _MetallicTex, IN.uv_MainTex ));
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Emission = tex2D (_EmissiveTex, IN.uv_MainTex) * _EmissiveAmount;
		}
		ENDCG
	}
	FallBack "Diffuse"
}


