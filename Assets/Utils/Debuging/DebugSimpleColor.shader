Shader "Custom/SimpleColor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture (optional)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            // kombinuj teksturu i boju (ako želiš samo boju, ukloni množenje sa tex)
            o.Albedo = (_Color.rgb) * tex.rgb;
            o.Alpha = _Color.a * tex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
