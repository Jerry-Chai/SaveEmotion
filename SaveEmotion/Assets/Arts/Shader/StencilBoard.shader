Shader "Unlit/StencilBoard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("BaseColor", Color) = (0,0,0,0)
    }
    SubShader
    {
       Tags { "Queue"="Geometry-150" }
        LOD 100

        Pass
        {

            //Zwrite Off
            Stencil
            {
                Ref 0
                Comp Equal
                Pass Replace
            }
            //cull front
            //ColorMask 0
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
            float4 _BaseColor;

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
                fixed4 col = tex2D(_MainTex, i.uv);
                //col *= _BaseColor;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                //return half4(1,1,1,0.0f);
                return col;
            }
            ENDCG
        }
    }
}
