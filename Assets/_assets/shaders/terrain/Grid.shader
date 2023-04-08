Shader "Custom/Grid"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GridColorX ("Grid Color X", Color) = (0,0,0,1)
        _GridColorY ("Grid Color Y", Color) = (0,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GridOpacity ("Grid Opacity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        fixed4 _Color;
        float4 _GridColorX;
        float4 _GridColorY;
        float _GridOpacity;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float3 color = c.rgb*_Color;
            
            float3 position = IN.worldPos;
            position = position % 1;

            float _step = step(0.1, position.x);
            _step = saturate(_step + (1-_GridOpacity));
            color *= (_step);
            float inverse = 1 - _step;
            color += _GridColorX*inverse;


            _step = step(0.1, position.z);
            _step = saturate(_step + (1-_GridOpacity));
            color *= (_step);
            inverse = 1 - _step;
            color += _GridColorY*inverse;


            o.Albedo = color;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
