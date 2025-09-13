Shader "Custom/SheenSweep"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5

        _SheenColor ("Sheen Color", Color) = (1,1,1,1)
        _SheenIntensity ("Sheen Intensity", Range(0,5)) = 1.0
        _SheenWidth ("Sheen Width", Range(0.01,1)) = 0.2
        _Speed ("Sheen Speed", Range(-5,5)) = 1.0
        _Direction ("Sweep Direction", Vector) = (1,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;
        half _Metallic;
        half _Smoothness;

        fixed4 _SheenColor;
        float _SheenIntensity;
        float _SheenWidth;
        float _Speed;
        float4 _Direction;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = tex.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = tex.a;

            // Projekcija pozicije na pravac
            float3 dir = normalize(_Direction.xyz);
            float proj = dot(IN.worldPos, dir);

            // Animacija pomeranjem preko vremena
            float bandPos = frac(proj + _Time.y * _Speed);

            // Traka oko vrednosti 0.5
            float band = smoothstep(0.5 - _SheenWidth, 0.5, bandPos) * 
                         (1.0 - smoothstep(0.5, 0.5 + _SheenWidth, bandPos));

            // Fresnel efekat da izgleda glossy
            float NdotV = saturate(dot(o.Normal, normalize(IN.viewDir)));
            float fresnel = pow(1.0 - NdotV, 3.0);

            float sheen = band * _SheenIntensity * (0.3 + 0.7 * fresnel);

            o.Emission = _SheenColor.rgb * sheen;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
