Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (0,0,0,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            fixed4 _Color;
            fixed4 _Color2;
            fixed4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_Color, _Color2, i.uv.x);
                fixed line1 = step(0.2, i.uv.y);
                fixed line2 = step(i.uv.y, 0.8);
                col = lerp(col, _OutlineColor, (1-line1*line2) * smoothstep(0.6,0,i.uv.x));
                return col;
            }
            ENDCG
        }
    }
}
