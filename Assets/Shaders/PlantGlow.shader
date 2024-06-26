Shader "Custom/PlantGlowShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _GlowSpeed ("Glow Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.texcoord);
                float glow = (sin(_Time.y * _GlowSpeed) + 1.0) * 0.5; // Oscillate between 0 and 1
                if (color.a > 0)
                {
                    color.rgb += _GlowColor.rgb * _GlowIntensity * glow;
                }
                return color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}