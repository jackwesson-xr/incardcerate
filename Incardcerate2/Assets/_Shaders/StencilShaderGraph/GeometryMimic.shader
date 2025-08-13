Shader "Custom/RepeatBehind_ToggleLocal"
{
    Properties
    {
        _Offset ("Offset Per Copy", Vector) = (0, 0, -0.2, 0)
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle(_USE_LOCAL_OFFSET)] _UseLocalOffset ("Use Local Offset", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            // Setup
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _USE_LOCAL_OFFSET

            // URP Core (defines all necessary macros)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Material properties
            CBUFFER_START(UnityPerMaterial)
                float3 _Offset;
                float4 _Color;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                uint index = UNITY_GET_INSTANCE_ID();

                float3 offset = _Offset * index;
                float3 worldPos;

                #ifdef _USE_LOCAL_OFFSET
                    float3 localPos = IN.positionOS + offset;
                    worldPos = TransformObjectToWorld(localPos);
                #else
                    float3 originalWorld = TransformObjectToWorld(IN.positionOS);
                    worldPos = originalWorld + offset;
                #endif

                Varyings OUT;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _Color;
            }

            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
