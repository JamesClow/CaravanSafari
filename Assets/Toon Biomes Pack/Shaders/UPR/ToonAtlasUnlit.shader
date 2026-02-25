Shader "ToonAtlasUnlit"
{
    Properties
    {
        _BaseMap ("Base Map (your atlas)", 2D) = "white" {}
        _BaseTint ("Base Tint", Color) = (1,1,1,1)

        _ShadowStrength ("Shadow Strength", Range(0,1)) = 1.0
        _AmbientStrength ("Ambient Strength", Range(0,1)) = 0.35
        _MinLight ("Min Light Floor", Range(0,1)) = 0.15
        [Toggle] _InvertMainLight ("Invert Main Light Direction", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // URP lighting keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseTint;
                float  _ShadowStrength;
                float  _AmbientStrength;
                float  _MinLight;
                float  _InvertMainLight;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            Varyings vert (Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   nrm = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = pos.positionCS;
                OUT.positionWS  = pos.positionWS;
                OUT.normalWS    = nrm.normalWS;
                OUT.uv          = IN.uv;
                OUT.shadowCoord = TransformWorldToShadowCoord(pos.positionWS);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).rgb * _BaseTint.rgb;

                float3 N = normalize(IN.normalWS);
                Light  mainLight = GetMainLight(IN.shadowCoord);
                float3 L = (_InvertMainLight > 0.5) ?  mainLight.direction : -mainLight.direction;

                float lambert = saturate(dot(N, L));

                float shadow  = lerp(1.0, mainLight.shadowAttenuation, _ShadowStrength);
                float direct  = max(lambert * shadow, _MinLight);

                float3 ambient = SampleSH(N) * _AmbientStrength;

                float3 color = albedo * (direct * mainLight.color + ambient);

                #ifdef _ADDITIONAL_LIGHTS
                uint cnt = GetAdditionalLightsCount();
                for (uint i = 0u; i < cnt; i++)
                {
                    Light l = GetAdditionalLight(i, IN.positionWS);
                    float lam = saturate(dot(N, -l.direction));
                    color += albedo * lam * l.color * l.distanceAttenuation * l.shadowAttenuation;
                }
                #endif

                return float4(color, 1.0);
            }
            ENDHLSL
        }

        // Reuse URP/Lit shadow caster to avoid include-path issues (works in previews too)
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }

    FallBack Off
}
