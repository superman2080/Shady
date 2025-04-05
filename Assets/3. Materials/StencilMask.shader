Shader "Custom/URP_StencilMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }

        Stencil
        {
            Ref 1
            Comp Equal // Stencil 값이 1인 경우에만 렌더링
        }

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(1, 1, 1, 1); // 기본 색상
            }
            ENDHLSL
        }
    }
}
