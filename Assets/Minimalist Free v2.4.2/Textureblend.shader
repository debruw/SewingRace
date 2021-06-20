Shader "Custom/TextureBlend" {
         Properties {
             _Color ("Color", Color) = (1,1,1,1)
             _Blend ("Texture Blend", Range(0,1)) = 0.0
             _MainTex ("Albedo (RGB)", 2D) = "white" {}
             _MainTex2 ("Albedo 2 (RGB)", 2D) = "white" {}
             _Glossiness ("Smoothness", Range(0,1)) = 0.5
             _Metallic ("Metallic", Range(0,1)) = 0.0
             _BumpMap ("Bumpmap", 2D) = "bump" {}
             _BumpMap2 ("Bumpmap", 2D) = "bump" {}
         }
         SubShader {
             Tags { "RenderType"="Opaque" }
             LOD 200
              Cull Off
              ZTest LEqual

             CGPROGRAM
             // Physically based Standard lighting model, and enable shadows on all light types
             #pragma surface surf Standard fullforwardshadows

             // Use shader model 3.0 target, to get nicer looking lighting
             #pragma target 3.0

             sampler2D _MainTex;
             sampler2D _MainTex2;
             sampler2D _BumpMap;
             sampler2D _BumpMap2;

             struct Input {
                 float2 uv_MainTex;
                 float2 uv_MainTex2;
                 float2 uv_BumpMap;
                 float2 uv_BumpMap2;
             };

             half _Blend;
             half _Glossiness;
             half _Metallic;
             fixed4 _Color;

             void surf (Input IN, inout SurfaceOutputStandard o) {
                 // Albedo comes from a texture tinted by color
                 fixed4 c = lerp (tex2D (_MainTex, IN.uv_MainTex), tex2D (_MainTex2, IN.uv_MainTex2), _Blend) * _Color;
                 o.Albedo = c.rgb;
                 // Metallic and smoothness come from slider variables
                 o.Metallic = _Metallic;
                 o.Smoothness = _Glossiness;
                 o.Alpha = c.a;
                 fixed4 n = lerp (tex2D (_BumpMap, IN.uv_BumpMap), tex2D (_BumpMap2, IN.uv_BumpMap2), _Blend) * _Color;
                 o.Normal = n.rgb;
             }
             ENDCG
         }
         FallBack "Diffuse"
     }