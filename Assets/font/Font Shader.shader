// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Font"
{
    Properties
    {
        //[PerRendererData] 
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Index 1", Color) = (1,0,0,1)
_Color2 ("Index 2", Color) = (1,0,0,1)
    }
  

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline" = "LightweightPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };
            
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            sampler2D _MainTex;
            
half4 _Color2;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                
                #ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
                #endif
                
                return OUT;
            }



            fixed4 frag(v2f IN) : SV_Target
            {
                half4 c = tex2D(_MainTex, IN.texcoord);
                int r = (int)(c.r * 255.0);
                if( r==1 )
                    c.rgb = lerp( _Color.rgb * min(1.0,c.g*2.0), float4(1,1,1,1), (c.g-0.5)*2.0 );
                if( r==2 )
                    c.rgb = lerp( _Color2.rgb * min(1.0,c.g*2.0), float4(1,1,1,1), (c.g-0.5)*2.0 );
                    
                //half4 emissive = (c + _FlashColor * 0.7) * _FlashAmount + SAMPLE_TEXTURE2D(_EmissiveTex, sampler_EmissiveTex, i.uv)  * _EmissiveAmount;
                return c * IN.color;
            }
        ENDCG
        }
    }
}