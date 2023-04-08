Shader "Custom/ToonOutlineTriplanar"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Texture", 2D) = "white" {}
		_TextureInfluence ("Texture Influence", Range(0, 1)) = 1
        _ToonColor ("Toon Color", Color) = (1, 1, 1, 1)
		_HColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1.0)
        _SColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1.0)

		_ToonSteps ("Steps of Toon", range(1, 9)) = 2
        _RampThreshold ("Ramp Threshold", Range(0.1, 1)) = 0.5
        _RampSmooth ("Ramp Smooth", Range(0, 1)) = 0.1

		_OutlineOpacity ("Outline Opacity", Range(0, 1)) = 1.0

		_FirstOutlineColor("Outline color", Color) = (1,0,0,0.5)
		_FirstOutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.15

		_Angle("Switch shader on angle", Range(0.0, 180.0)) = 89

		_MainTexSide("Side/Bottom Texture", 2D) = "white" {}
		_TopScale("Top Scale", Range(-2,2)) = 1
		_SideScale("Side Scale", Range(-2,2)) = 1
		_TopSpread("TopSpread", Range(-2,8)) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

	uniform float4 _FirstOutlineColor;
	uniform float _FirstOutlineWidth;

	float _OutlineOpacity;

	uniform sampler2D _MainTex;
	uniform float4 _Color;
	uniform float _Angle;

	ENDCG

	SubShader{
		//First outline
		Pass{
			Tags{ "Queue" = "Geometry" }
			Cull Front
			CGPROGRAM

			struct v2f {
				float4 pos : SV_POSITION;
			};

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v) {
				appdata original = v;

				float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
				//This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
				//If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
				//Otherwise the old-school normal vector scaling is used
				//This way prevents weird artifacts from being created when using either of the methods
				if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
					v.vertex.xyz += normalize(v.normal.xyz) * _FirstOutlineWidth * _OutlineOpacity;
				}else {
					v.vertex.xyz += scaleDir * _FirstOutlineWidth * _OutlineOpacity;
				}

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR{
				float4 color = _FirstOutlineColor;
				color.a = 1;
				return color;
			}

			ENDCG
		}
		
		//Surface shader
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Toon addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

		sampler2D _MainTexSide;
        fixed4 _ToonColor;
        fixed4 _HColor;
        fixed4 _SColor;
        
        float _RampThreshold;
        float _RampSmooth;
        float _ToonSteps;
        float _TopScale, _SideScale;
        float _TextureInfluence;
        float  _TopSpread;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
	        float3 worldNormal;
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
			/*
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            mainTex *= _TextureInfluence;

            float3 color  = mainTex.rgb + _ToonColor.rgb;

            o.Albedo = color;
            o.Alpha = mainTex.a * _ToonColor.a;
			*/
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex*_TopScale);
            mainTex *= _TextureInfluence;

            float3 toptexture = mainTex.rgb * _ToonColor;

			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));
            // triplanar for side and bottom texture, x,y,z sides
			float3 x = tex2D(_MainTexSide, IN.worldPos.zy * _SideScale);
			float3 y = tex2D(_MainTexSide, IN.worldPos.zx * _SideScale);
			float3 z = tex2D(_MainTexSide, IN.worldPos.xy * _SideScale);

			// lerped together all sides for side bottom texture
			float3 sidetexture = z;
			sidetexture = lerp(sidetexture, x, blendNormal.x);
			sidetexture = lerp(sidetexture, y, blendNormal.y);

			float worldNormalDotNoise = dot(o.Normal, IN.worldNormal.y);
            
            float3 topTextureResult = step(_TopSpread, worldNormalDotNoise) * toptexture;
			// if dot product is lower than the top spread slider, multiplied by triplanar mapped side/bottom texture
			float3 sideTextureResult = step(worldNormalDotNoise, _TopSpread) * sidetexture;
			float3 _diffuseSide = topTextureResult + sideTextureResult;

            float3 colorEnd = _diffuseSide;
            
            o.Albedo = colorEnd;
        }

		ENDCG
	}
	Fallback "Diffuse"
}
