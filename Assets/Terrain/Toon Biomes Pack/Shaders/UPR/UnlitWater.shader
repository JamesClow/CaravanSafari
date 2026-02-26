Shader "UnlitWater"
{
    Properties
    {
        _WaterColorNear     ("Depth Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _WaterColorDeep     ("Water Color", Color) = (0.086, 0.407, 1, 0.749)
        _WaterMaxDepth      ("Visible Depth", Float) = 1.0

        _ShorelineFoamColor ("Water Outline Color", Color) = (1,1,1,1)

        [NoScaleOffset]_WaveDistortionMap ("Surface Noise", 2D) = "white" {}

        [Toggle(_USEWORLDTILING_ON)] _UseWorldTiling ("Lock Tiling to World Space", Float) = 0
        _WorldTileSize   ("World Tile Size (m)", Float) = 1.0
        _WorldTileScale  ("World Tiling Scale", Float) = 1.0

        _WaveScrollSpeed ("Scroll Speed (uv/s)", Float) = 0.01

        _FoamDistanceMax ("Water Outline Max", Float) = 0.45
        _FoamDistanceMin ("Water Outline Min", Float) = 0.30
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   2.0

            // Keyword for world-tiling toggle
            #pragma shader_feature _USEWORLDTILING_ON

            // Enable GPU instancing (compatible with SRP Batcher)
            #pragma multi_compile_instancing

            #define SMOOTHSTEP_AA 0.01

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex         : SV_POSITION;
                float2 uv0            : TEXCOORD0;
                float4 screenPos      : TEXCOORD1;
                float3 viewNormal     : TEXCOORD2;
                float3 worldPos       : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ===== Textures / Samplers (not per-material constants) =====
            sampler2D _WaveDistortionMap;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;

            // ===== Per-material constants for SRP Batcher =====
            CBUFFER_START(UnityPerMaterial)
                float4 _WaterColorNear;
                float4 _WaterColorDeep;
                float4 _ShorelineFoamColor;

                float  _WaterMaxDepth;
                float  _FoamDistanceMax;
                float  _FoamDistanceMin;
                float  _WaveScrollSpeed;

                float  _WorldTileSize;
                float  _WorldTileScale;
                float  _UseWorldTiling; // toggle as float
            CBUFFER_END

            // Editor-time time input (set externally from EditorTimeDriver.cs)
            float _EditorTime; // Not per-material — declared outside CBUFFER

            inline float4 AlphaBlend(float4 top, float4 bottom)
            {
                float3 color = top.rgb * top.a + bottom.rgb * (1 - top.a);
                float  alpha = top.a + bottom.a * (1 - top.a);
                return float4(color, alpha);
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex    = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);

                // View-space normal instead of COMPUTE_VIEW_NORMAL
                float3 worldN = UnityObjectToWorldNormal(v.normal);
                float3 viewN  = mul((float3x3)UNITY_MATRIX_V, worldN);
                o.viewNormal  = normalize(viewN);

                o.uv0      = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // ---------- Depth gradient with fallback ----------
                float rawDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
                bool  hasDepth = (rawDepth < 0.9999);

                // Linear eye depth of the background scene behind the water surface
                float depthLin  = hasDepth ? LinearEyeDepth(rawDepth) : (i.screenPos.w + _WaterMaxDepth);
                // Distance from the water surface to background geometry
                float depthDiff = depthLin - i.screenPos.w;

                float waterLerp = saturate(depthDiff / max(_WaterMaxDepth, 1e-5));
                float4 waterCol = lerp(_WaterColorNear, _WaterColorDeep, waterLerp);

                // ---------- Foam using scene normals (fallback if no normal data) ----------
                float3 nRaw = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPos)).xyz;
                float  nSum = nRaw.x + nRaw.y + nRaw.z;

                float nDot = 1.0;
                if (nSum > 1e-5)
                {
                    float3 bgN = normalize(nRaw * 2.0 - 1.0); // remap [0..1] → [-1..1]
                    nDot = saturate(dot(bgN, i.viewNormal));
                }

                float foamDist = lerp(_FoamDistanceMax, _FoamDistanceMin, nDot);
                float cutoff   = saturate(depthDiff / max(foamDist, 1e-5));

                // ---------- Safe time for editor preview (fallback to _Time.y if needed) ----------
                float t = (_EditorTime > 0.0001) ? _EditorTime : _Time.y;
                float2 scroll = t * _WaveScrollSpeed.xx;

                // ---------- UV selection: world-tiling or mesh UV ----------
                float2 baseUV;
                #if defined(_USEWORLDTILING_ON)
                    float tileSize  = max(_WorldTileSize, 1e-5);
                    float safeScale = max(abs(_WorldTileScale), 0.01) * sign(_WorldTileScale);
                    baseUV = (i.worldPos.xz / tileSize) * safeScale;
                #else
                    baseUV = i.uv0;
                #endif

                float2 uv    = baseUV + frac(scroll);
                float  noise = tex2D(_WaveDistortionMap, uv).r;

                float foam = smoothstep(cutoff - SMOOTHSTEP_AA,
                                        cutoff + SMOOTHSTEP_AA,
                                        noise);

                float4 foamCol = _ShorelineFoamColor;
                foamCol.a *= foam;

                return AlphaBlend(foamCol, waterCol);
            }
            ENDCG
        }
    }
}
