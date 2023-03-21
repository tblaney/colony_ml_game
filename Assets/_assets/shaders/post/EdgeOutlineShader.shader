Shader "Hidden/EdgeOutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MapThreshold ("Threshold", Range(0, 1)) = 0.1

        _BlurSize("Blur Size", Range(0,0.1)) = 0
        _StandardDeviation("Standard Deviation (Gauss only)", Range(0, 0.1)) = 0.02
    }

    CGINCLUDE

        float4 GetColorBlurred(float4 mainTexelSize ) {
            return float4(0, 0, 0, 0);
        }

    ENDCG

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
            #define PI 3.14159265359
            #define E_1 2.71828182846
            #define SAMPLES 10

            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _StandardDeviation;
            float _MapThreshold;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                if (_StandardDeviation == 0)
                    return tex2D(_MainTex, i.uv);

                fixed4 col_C = 0;
                fixed4 col_N = 0;
                fixed4 col_S = 0;
                fixed4 col_W = 0;
                fixed4 col_E = 0;


                for (int idx = 0; idx < 5; idx++) 
                {
                    float4 col = 0;
                    float sum = 0;

                    float2 uv_offset = float2(0, 0);
                    if (idx == 1)
                        uv_offset = float2(0, _MainTex_TexelSize.y);
                    else if (idx == 2)
                        uv_offset = -float2(0, _MainTex_TexelSize.y);
                    else if (idx == 3) 
                        uv_offset = float2(_MainTex_TexelSize.x, 0);
                    else if (idx == 4) 
                        uv_offset = -float2(_MainTex_TexelSize.x, 0);

                    for (float index = 0; index < SAMPLES; index++)
                    {
                        //get the offset of the sample
                        float offset = (index / (SAMPLES - 1) - 0.5) * _BlurSize;
                        //get uv coordinate of sample
                        float2 uv = (i.uv + uv_offset) + float2(0, offset);

                        //calculate the result of the gaussian function
                        float stDevSquared = _StandardDeviation * _StandardDeviation;
                        float gauss = (1 / sqrt(2 * PI * stDevSquared)) * pow(E_1, -((offset * offset) / (2 * stDevSquared)));
                        //add result to sum
                        sum += gauss;
                        //multiply color with influence from gaussian function and add it to sum color
                        col += tex2D(_MainTex, uv) * gauss;
                    }
                    //divide the sum of values by the amount of samples
                    col = col / sum;

                    if (idx == 0) 
                        col_C = col;
                    else if (idx == 1) 
                        col_N = col;
                    else if (idx == 2) 
                        col_S = col;                
                    else if (idx == 3) 
                        col_W = col;    
                    else if (idx == 4) 
                        col_E = col;
                    
                }

                //col.rgb = 1 - col.rgb;

                // edge detection:
                float3 C = col_C.rgb;
                float3 N = col_N.rgb;
                float3 S = col_S.rgb;
                float3 W = col_W.rgb;
                float3 E = col_E.rgb;

                float C_lum = LUM(C);
                float N_lum = LUM(N);
                float S_lum = LUM(S);
                float W_lum = LUM(W);
                float E_lum = LUM(E);


                // laplacian
                float L_lum = saturate(N_lum + S_lum + W_lum + E_lum - 4 * C_lum);

                L_lum = step(_MapThreshold, L_lum);

                float4 color = (1-tex2D(_MainTex, i.uv)) + float4(L_lum, L_lum, L_lum, 1);
                color.rgb = 1 - color.rgb;

                return color;
            }
            ENDCG
        }
        /*
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define LUM(c) ((c).r*.3 + (c).g*.59 + (c).b*.11)

            float4 _MainTex_TexelSize;
            float _MapThreshold;

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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;


                // edge detection:
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


                float4 color = col + float4(L_lum, L_lum, L_lum, 1);
                color.rgb = 1 - color.rgb;

                return color;
            }
            ENDCG
        }
        */
    }
}
