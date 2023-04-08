Shader "Hidden/EdgeOutlineBase"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MapThreshold ("Threshold", Range(0, 1)) = 0.1
        _ColorThreshold ("Threshold", Range(0, 1)) = 1
        
        _Color("Highlight Color", Color) = (1, 1, 1, 1)
        _RT ("Highlight Texture", 2D) = "black" {}


    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define LUM(c) ((c).r*.3 + (c).g*.59 + (c).b*.11)

            float4 _MainTex_TexelSize;
            float _MapThreshold;
            float4 _Color;
            float _ColorThreshold;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _RT;


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 colRT = tex2D(_RT, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;

                float3 C = tex2D(_MainTex, i.uv).rgb;
                float3 N = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).rgb;
                float3 S = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).rgb;
                float3 W = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).rgb;
                float3 E = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).rgb;


                float C_lum = LUM(C);
                float N_lum = LUM(N);
                float S_lum = LUM(S);
                float W_lum = LUM(W);
                float E_lum = LUM(E);


                // laplacian
                float L_lum = saturate(N_lum + S_lum + W_lum + E_lum - 4 * C_lum);

                L_lum = step(_MapThreshold, L_lum);

                //L_lum = 1 - L_lum;
                float4 color = col - _ColorThreshold*float4(L_lum, L_lum, L_lum, 1);

                color += saturate(colRT-(1-_Color));
                //float4 color = float4(L_lum, L_lum, L_lum, 1);

                //color.rgb = 1 - color.rgb;

                return color;
            }
            ENDCG
        }
    }
}
