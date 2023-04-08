Shader "GPUInstancer/Toon/Basic/ToonBase"
{
    Properties
    {
        // Colors
        _Color ("Color", Color) = (1, 1, 1, 1)
        _HColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1.0)
        _SColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1.0)

        // texture
        _MainTex ("Main Texture", 2D) = "white" { }
        _TextureInfluence ("Texture Influence", Range(0, 1)) = 1

        // ramp
        _ToonSteps ("Steps of Toon", range(1, 9)) = 2
        _RampThreshold ("Ramp Threshold", Range(0.1, 1)) = 0.5
        _RampSmooth ("Ramp Smooth", Range(0, 1)) = 0.1

    
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
        
        CGPROGRAM
#include "UnityCG.cginc"
#include "./../../../GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing

        #pragma surface surf Toon addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        
        fixed4 _Color;
        fixed4 _HColor;
        fixed4 _SColor;
        
        sampler2D _MainTex;
        
        float _RampThreshold;
        float _RampSmooth;
        float _ToonSteps;

        float _TextureInfluence;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };
        
        float linearstep(float min, float max, float t)
        {
            return saturate((t - min) / (max - min));
        }
        
        inline fixed4 LightingToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
            half3 normalDir = normalize(s.Normal);
            half3 halfDir = normalize(lightDir + viewDir);
            
            float ndl = max(0, dot(normalDir, lightDir));
            float ndh = max(0, dot(normalDir, halfDir));
            float ndv = max(0, dot(normalDir, viewDir));
            
            // multi steps
            float diff = smoothstep(_RampThreshold - ndl, _RampThreshold + ndl, ndl);
            float interval = 1 / _ToonSteps;
            // float ramp = floor(diff * _ToonSteps) / _ToonSteps;
            float level = round(diff * _ToonSteps) / _ToonSteps;
            float ramp ;
            if (_RampSmooth == 1)
            {
                ramp = interval * linearstep(level - _RampSmooth * interval * 0.5, level + _RampSmooth * interval * 0.5, diff) + level - interval;
            }
            else
            {
                ramp = interval * smoothstep(level - _RampSmooth * interval * 0.5, level + _RampSmooth * interval * 0.5, diff) + level - interval;
            }
            ramp = max(0, ramp);
            ramp *= atten;
            
            _SColor = lerp(_HColor, _SColor, _SColor.a);
            float3 rampColor = lerp(_SColor.rgb, _HColor.rgb, ramp);
        
            
            fixed3 lightColor = _LightColor0.rgb;
            
            fixed4 color;
            fixed3 diffuse = s.Albedo * lightColor * rampColor;
            
            color.rgb = diffuse;
            color.a = s.Alpha;
            return color;
        }
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            mainTex *= _TextureInfluence;

            float3 color  = mainTex.rgb + _Color.rgb;

            o.Albedo = color; 
            o.Alpha = mainTex.a * _Color.a;
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
