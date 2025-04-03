Shader "Custom/GaussianBlurUI"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 50)) = 10.0 // Blur intenzitás
        _Direction ("Blur Direction", Vector) = (1, 0, 0, 0) // (1,0) vízszintes, (0,1) függõleges
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _BlurSize;
            float2 _Direction; // A blur iránya: vízszintes vagy függõleges

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Texel méret számítása
                float2 texelSize = _BlurSize / float2(_ScreenParams.x, _ScreenParams.y);

                // Gaussian minták súlyozott összege
                half4 color = tex2D(_MainTex, i.uv) * 0.2; // Középsõ minta nagyobb súllyal

                // Több minta hozzáadása kisebb súlyozással
                color += tex2D(_MainTex, i.uv + _Direction * texelSize * 1.0) * 0.15;
                color += tex2D(_MainTex, i.uv - _Direction * texelSize * 1.0) * 0.15;

                color += tex2D(_MainTex, i.uv + _Direction * texelSize * 2.0) * 0.1;
                color += tex2D(_MainTex, i.uv - _Direction * texelSize * 2.0) * 0.1;

                color += tex2D(_MainTex, i.uv + _Direction * texelSize * 3.0) * 0.05;
                color += tex2D(_MainTex, i.uv - _Direction * texelSize * 3.0) * 0.05;

                return color;
            }
            ENDCG
        }
    }
}
