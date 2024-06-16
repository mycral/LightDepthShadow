Shader "Unlit/DepthTextureX"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float4 vertex : SV_POSITION;
                float z : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.z = o.vertex.z/o.vertex.w;
                return o;
            }

            half frag (v2f i) : SV_Target
            {
                #if UNITY_REVERSED_Z
                    return 1 - i.z;
                #else
                    return (i.z + 1) * 0.5;
                #endif
            }
            ENDCG
        }
    }
}
