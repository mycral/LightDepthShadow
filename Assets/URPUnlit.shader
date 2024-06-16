// This shader fills the mesh shape with a color predefined in the code.
Shader "Depth/URPUnlitShader"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                float4 positionOS :  POSITION;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 depthClipPos : TEXCOORD0;
            };            

             float4x4 _gLightVP;
            
             TEXTURE2D(_gDepthTex);
             SAMPLER(sampler_gDepthTex);

            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                OUT.depthClipPos = mul(_gLightVP, float4(TransformObjectToWorld(float3(0,0,0)),1));

                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                 float3 ndcPos = IN.depthClipPos.xyz / IN.depthClipPos.w;
                 ndcPos.xy = (ndcPos.xy + 1) *0.5;
#if UNITY_UV_STARTS_AT_TOP
                ndcPos.y = 1 - ndcPos.y;
#endif
#if UNITY_REVERSED_Z
                float depthVertex = 1 - ndcPos.z;
#else
                float depthVertex = (ndcPos.z + 1) * 0.5;
#endif
                float depthOnTex = PLATFORM_SAMPLE_TEXTURE2D(_gDepthTex,sampler_gDepthTex, ndcPos.xy).r;
                if(depthVertex > depthOnTex + 0.001)
			     {
				     return 0;
			     }
                return half4(0.5, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}