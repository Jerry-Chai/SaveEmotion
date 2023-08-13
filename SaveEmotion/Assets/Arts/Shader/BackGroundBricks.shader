// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/BackGroundBricks"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+30"}
        LOD 100
        Zwrite Off
        Cull back
        //Blend One OneMinusSrcAlpha
        Blend DstColor Zero, Zero One
        //Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _BaseColor;
            half4 _WorldPos;

            v2f vert (appdata v)
            {
                v2f o;
                half3 worldPos = mul(UNITY_MATRIX_MV, float4(v.vertex.xyz, 1.0)).xyz;
                worldPos.y += sin(_Time.y + _WorldPos.x + _WorldPos.y) * 3.0;
                o.vertex = mul(UNITY_MATRIX_P, float4(worldPos.xyz, 1.0));

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                      
                col = 1.0f;
                col *= _BaseColor;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                
                //col.a *= _BaseColor.a;
                return col;
            }
            ENDCG
        }
    }
}
