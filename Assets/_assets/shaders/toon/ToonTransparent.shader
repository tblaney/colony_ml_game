// surface shader version of https://forum.unity.com/threads/gltfutility-a-simple-gltf-plugin.654319/page-4#post-6854009

// how to enable transparency for surface shaders
// https://forum.unity.com/threads/transparency-with-standard-surface-shader.394551/
// https://forum.unity.com/threads/simply-adding-alpha-fade-makes-my-shader-only-work-in-scene-view.546852/

Shader "Toon/Basic/Transparent"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Opacity ("Opacity", Range(0, 1)) = 1

		// Colors
        _HColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1.0)
        _SColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1.0)
        
        // texture
        _MainTex ("Main Texture", 2D) = "white" { }
        _TextureInfluence ("Texture Influence", Range(0, 1)) = 1

        // ramp
        _ToonSteps ("Steps of Toon", range(1, 9)) = 2
        _RampThreshold ("Ramp Threshold", Range(0.1, 1)) = 0.5
        _RampSmooth ("Ramp Smooth", Range(0, 1)) = 0.1

		[Toggle] _PerformHide("Perform Hide", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200

		Pass {
			ZWrite On
			Cull Off // make double sided
			ColorMask 0 // don't draw any color

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
			float4 _MainTex_ST;

			

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - .97); // remove non-opaque pixels from writing to zbuffer
				return col;
			}
			ENDCG
		}

		// ---------- Start Pass 2 ----------
		ZWrite Off
		Cull Off // make double sided
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Toon fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;

		fixed4 _HColor;
		fixed4 _SColor;
		
		float _RampThreshold;
		float _RampSmooth;
		float _ToonSteps;

		float pi = 3.141592653589793238462;

		float _Opacity;
		float _TextureInfluence;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

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

		void surf (Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			c *= _TextureInfluence;
			o.Albedo = c.rgb + _Color.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			float alpha = _Opacity;

			o.Alpha = alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
