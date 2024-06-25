Shader "Custom/HitFlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(1, 100)) = 10
        _FlickerSpeed ("Flicker Speed", Range(1, 50)) = 20
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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _FlashColor;
            float _FlashAmount;
            sampler2D _NoiseTex;
            float _NoiseScale;
            float _FlickerSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float2 noiseUV = i.uv * _NoiseScale;
                float flicker = tex2D(_NoiseTex, noiseUV + _Time.y * _FlickerSpeed).r;
                float flash = _FlashAmount * flicker;
                texColor = lerp(texColor, _FlashColor, flash);
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
