Shader "Custom/index-sprite-bump" {
	Properties {
		_Color ("Multiply Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_BlendColor ("Blend Color", Color) = (1,1,1,1)
		_BlendAmount("Blend Alpha", Range(0.0, 1.0)) = 0

		_IndexColor ("Index 1", Color) = (1,0,0,1)
		_IndexColor2 ("Index 2", Color) = (1,0,0,1)

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicTex ("Metallic (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0.0,1.0)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissiveTex ("Emissive (RGB)", 2D) = "white" {}
		_EmissiveAmount ("Amount", Range(0,1)) = 0.0

		_BumpMap ("Normalmap", 2D) = "bump" {}
        
        _FlipX ("FlipX", int) = 0 
	}
	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"RenderType"="Transparent" 
		}
		LOD 200
		Cull Off
		//Lighting Off
		//ZWrite Off
		//Blend One OneMinusSrcAlpha

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
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		half _EmissiveAmount;
		fixed4 _Color;
		fixed4 _IndexColor;
		fixed4 _IndexColor2;

		fixed4 _BlendColor;
		half _BlendAmount;
        
        int _FlipX;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float4 c = tex2D (_MainTex, IN.uv_MainTex);

			int r = (int)(c.r * 255.0);
			if( r==1 )
				c.rgb = lerp( _IndexColor.rgb * min(1.0,c.g*2.0), float4(1,1,1,1), (c.g-0.5)*2.0 );
			if( r==2 )
				c.rgb = lerp( _IndexColor2.rgb * min(1.0,c.g*2.0), float4(1,1,1,1), (c.g-0.5)*2.0 );


			o.Albedo = lerp( c.rgb, _BlendColor, _BlendAmount );
            float3 norm = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            if( _FlipX )
                norm.x = -norm.x;
            o.Normal = norm;
			o.Metallic = _Metallic * (1.0 - tex2D( _MetallicTex, IN.uv_MainTex ));
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Emission = tex2D (_EmissiveTex, IN.uv_MainTex) * _EmissiveAmount;
		}
		ENDCG
	}
	FallBack "Diffuse"
}


