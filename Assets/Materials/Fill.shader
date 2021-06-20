// animated scrolling texture with fill amount
// https://unitycoder.com/blog/2020/03/13/shader-scrolling-texture-with-fill-amount/

Shader "Fill"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Fill("Fill", Range(0.0,1.0)) = 0.5
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        Pass
        {
            // Enable alpha-to-coverage mode for this SubShader
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"   
            
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // discard if uv.y is below below cut value
                clip(step(i.uv.y, _Fill * _MainTex_ST.y)-0.1);

                return col * _Color;
            }   
            ENDCG
        }
    }
}