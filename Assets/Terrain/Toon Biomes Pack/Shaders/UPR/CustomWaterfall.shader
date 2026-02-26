Shader "Custom/Waterfall"
{
    Properties
    {
        // ---- Static (background, no scroll) ----
        _StaticTex("Static Texture", 2D) = "white" {}
        _StaticBottomColor("Static Bottom Color", Color) = (1,1,1,1)
        _StaticTopColor("Static Top Color", Color) = (1,1,1,1)
        _StaticOpacity("Static Opacity (0–100)", Range(0,100)) = 100

        // ---- Main Layer ----
        _MainTex("Main Texture", 2D) = "white" {}
        _MainTexSpeed("Main Speed (X,Y)", Vector) = (0.0, -1.0, 0, 0)
        _MainRotation("Main Scroll Rotation", Range(0,360)) = 0
        _MainTint("Main Tint", Color) = (1,1,1,1)
        _MainOpacity("Main Opacity (0–100)", Range(0,100)) = 100

        // ---- Second Layer (top) ----
        _SecondTex("Second Texture", 2D) = "white" {}
        _SecondTexSpeed("Second Speed (X,Y)", Vector) = (0.0, -1.0, 0, 0)
        _SecondRotation("Second Scroll Rotation", Range(0,360)) = 0
        _SecondTint("Second Tint", Color) = (1,1,1,1)
        _SecondOpacity("Second Opacity (0–100)", Range(0,100)) = 100

        // ---- Mask for both Main & Second ----
        _SecondMask("Alpha Mask (R=alpha for Main & Second)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f {
                float4 pos:SV_POSITION;
                float2 uvStatic:TEXCOORD0;
                float2 uvMain  :TEXCOORD1;
                float2 uvSecond:TEXCOORD2;
                float2 uvMask  :TEXCOORD3;
            };

            // Static
            sampler2D _StaticTex;  float4 _StaticTex_ST;
            float4 _StaticBottomColor;
            float4 _StaticTopColor;
            float  _StaticOpacity;

            // Main
            sampler2D _MainTex;    float4 _MainTex_ST;    float4 _MainTexSpeed;    float _MainRotation;
            float4 _MainTint;      float  _MainOpacity;

            // Second
            sampler2D _SecondTex;  float4 _SecondTex_ST;  float4 _SecondTexSpeed;  float _SecondRotation;
            float4 _SecondTint;    float  _SecondOpacity;

            // Mask
            sampler2D _SecondMask; float4 _SecondMask_ST;

            float2 RotateUV(float2 uv, float2 center, float deg)
            {
                float r = radians(deg); float s = sin(r), c = cos(r);
                uv -= center;
                uv = float2(uv.x*c - uv.y*s, uv.x*s + uv.y*c);
                return uv + center;
            }

            float4 Over(float4 top, float4 bottom)
            {
                float3 topP = top.rgb * top.a;
                float3 botP = bottom.rgb * bottom.a;
                float  a = top.a + bottom.a * (1 - top.a);
                float3 p = topP + botP * (1 - top.a);
                float3 rgb = (a > 0) ? (p / a) : 0;
                return float4(rgb, a);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Static
                o.uvStatic = TRANSFORM_TEX(v.uv, _StaticTex);

                // Main
                float2 uvMain = TRANSFORM_TEX(v.uv, _MainTex) + _Time.y * _MainTexSpeed.xy;
                o.uvMain = RotateUV(uvMain, float2(0.5,0.5), _MainRotation);

                // Second
                float2 uvSec = TRANSFORM_TEX(v.uv, _SecondTex) + _Time.y * _SecondTexSpeed.xy;
                o.uvSecond = RotateUV(uvSec, float2(0.5,0.5), _SecondRotation);

                // Mask (свой Tiling/Offset)
                o.uvMask = TRANSFORM_TEX(v.uv, _SecondMask);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Static с градиентом по UV.y
                fixed4 st = tex2D(_StaticTex, i.uvStatic);
                float  t = saturate(i.uvStatic.y);
                float3 grad = lerp(_StaticBottomColor.rgb, _StaticTopColor.rgb, t);
                st.rgb *= grad;
                st.a    = saturate(st.a * (_StaticOpacity/100.0) * lerp(_StaticBottomColor.a, _StaticTopColor.a, t));

                // Общая маска (R-канал)
                float mask = tex2D(_SecondMask, i.uvMask).r;
                mask = saturate(mask);

                // Main
                fixed4 m  = tex2D(_MainTex, i.uvMain);
                m.rgb *= _MainTint.rgb;
                m.a    = saturate(m.a * (_MainOpacity/100.0) * _MainTint.a);
                m.a   *= mask;                 // <— mask affects Main

                // Second
                fixed4 s  = tex2D(_SecondTex, i.uvSecond);
                s.rgb *= _SecondTint.rgb;
                s.a    = saturate(s.a * (_SecondOpacity/100.0) * _SecondTint.a);
                s.a   *= mask;                 // <— mask affects Second

                // Слои: Main over Static, затем Second over result
                float4 baseCol = Over(m, st);
                float4 outCol  = Over(s, baseCol);
                return outCol;
            }
            ENDCG
        }
    }
}
