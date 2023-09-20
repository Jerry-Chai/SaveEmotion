Shader "Unlit/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _BaseColor ("BaseColor", Color) = (0,0,0,0)
        _NoiseTexture ("NoiseTexture", 2D) = "white" {}
        _ControlValue ("_ControlValue", Float) = 5

        [HDR]_Emission ("_Emission", Color) =(0,0,0,0)
        _EdgeWidth ("_EdgeWidth", Float) = 2.0
        _DissolveDirection ("_DissolveDirection", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha 
        Stencil
        {
            Ref 1
            Comp Equal
            Pass Keep
        }
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
            float _ControlValue;
            float _EdgeWidth;

            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            float4 _BaseColor;
            float4 _Emission;
            float4 _DissolveDirection;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uvToCenter = i.uv - 0.5f;
                uvToCenter = uvToCenter * 2.0f;
                fixed4 col = tex2D(_MainTex, i.uv);
                // negative _DissolveDirection beacause of bricks orintation
                float currPos = dot(-_DissolveDirection.xy, uvToCenter);
                // to 0 - 1;
                currPos = (currPos + 1) /2.0f;
                currPos *= 200.0f;
                float2 noiseUV = i.uv * _NoiseTexture_ST.xy + _NoiseTexture_ST.zw;
                half4 noiseTex =  tex2D(_NoiseTexture, noiseUV) * 50.0f;
                half alpha = currPos - noiseTex - _ControlValue;
                alpha = saturate(alpha);

                float edge = 3.0f;
                float negative = alpha > 0.0f ? 1 : 0; 
                float largerthanValue = alpha < _EdgeWidth ? 1 : 0;
                edge =  edge * negative * largerthanValue;
                edge = edge > 0 ? 1 : 0;
                
                col *= _BaseColor;
                col.xyz += edge * _Emission.xyz;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return half4(col.xyz, alpha);
            }
            ENDCG
        }
    }
}
