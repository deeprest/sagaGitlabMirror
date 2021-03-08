Shader "Custom/2D/Sprite-Lit-Emissive Indexed"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        //_MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _EmissiveTex ("Emissive", 2D) = "white" {}

_EmissiveAmount ("Amount", Range(0,1)) = 0.0
//_IndexColor ("Index 1", Color) = (1,0,0,1)
//_IndexColor2 ("Index 2", Color) = (1,0,0,1)
_FlashColor ("Flash Color", Color) = (1,1,1,1)
_FlashAmount("Flash", Range(0.0, 1.0)) = 0

    }
    
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL
    
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "LightweightPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Lightweight2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
                float2	lightingUV  : TEXCOORD1;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            //TEXTURE2D(_MaskTex);
            //SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _MainTex_ST;
            half4 _NormalMap_ST;
            TEXTURE2D(_EmissiveTex);
            SAMPLER(sampler_EmissiveTex);

half _EmissiveAmount;
half4 _IndexColor;
half4 _IndexColor2;
half4 _FlashColor;
half _FlashAmount;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif
            
            float4 _IndexColors[2];

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;
                o.color = v.color;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"


            void GetIndexColor( inout half4 index )
            {
                int r = (int)(index.r * 255.0);
                if( r==1 ) index.rgb = lerp( _IndexColors[0].rgb * min(1.0,index.g*2.0), float3(1,1,1), (index.g-0.5)*2.0 );
                if( r==2 ) index.rgb = lerp( _IndexColors[1].rgb * min(1.0,index.g*2.0), float3(1,1,1), (index.g-0.5)*2.0 );
            }

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                GetIndexColor( c );
                //int r = (int)(c.r * 255.0);
                //if( r==1 ) c.rgb = lerp( _IndexColors[0].rgb * min(1.0,c.g*2.0), float3(1,1,1), (c.g-0.5)*2.0 );
                //if( r==2 ) c.rgb = lerp( _IndexColors[1].rgb * min(1.0,c.g*2.0), float3(1,1,1), (c.g-0.5)*2.0 );
                     
                    
                half4 etex = SAMPLE_TEXTURE2D(_EmissiveTex, sampler_EmissiveTex, i.uv);
                GetIndexColor( etex );
                
                //int e = (int)(etex.r * 255.0);
                //if( e==1 ) etex.rgb = lerp( _IndexColors[0].rgb * min(1.0,etex.g*2.0), float3(1,1,1), (etex.g-0.5)*2.0 );
                //if( e==2 ) etex.rgb = lerp( _IndexColors[1].rgb * min(1.0,etex.g*2.0), float3(1,1,1), (etex.g-0.5)*2.0 );
                
                half4 flash = (c + _FlashColor * 0.7) * _FlashAmount; //lerp(light, light+_FlashColor*0.7, _FlashAmount);
                half4 emissive = flash + etex  * _EmissiveAmount;
                half4 main = c;
                //half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                half4 light = CombinedShapeLightShared(main, main, i.lightingUV);
                return (light + emissive) * main.a;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
                float3  normalWS		: TEXCOORD1;
                float3  tangentWS		: TEXCOORD2;
                float3  bitangentWS		: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.uv = attributes.uv;
                o.color = attributes.color;
                o.normalWS = TransformObjectToWorldDir(float3(0, 0, 1));
                o.tangentWS = TransformObjectToWorldDir(float3(1, 0, 0));
                o.bitangentWS = TransformObjectToWorldDir(float3(0, 1, 0));
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            float4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, -i.normalWS.xyz);
            }
            ENDHLSL
        }
        Pass
        {
            Tags { "LightMode" = "LightweightForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return mainTex;
            }
            ENDHLSL
        }
    }
    Fallback "Hidden/Sprite-Fallback"
}
