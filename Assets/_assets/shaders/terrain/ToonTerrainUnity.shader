// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Edits by Glynn Taylor. MIT license
// Includes code for splitmap by https://twitter.com/adamgryu and triplanar mapping by https://github.com/keijiro. MIT License

Shader "Toon/TerrainUnity" 
{
    Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Top Texture", 2D) = "white" {}
		_MainTexSide("Side/Bottom Texture", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_Normal("Normal/Noise", 2D) = "bump" {}
		_Scale("Top Scale", Range(-2,2)) = 1
		_SideScale("Side Scale", Range(-2,2)) = 1
		_NoiseScale("Noise Scale", Range(-2,2)) = 1
		_TopSpread("TopSpread", Range(-2,8)) = 1
		_TopThreshold("TopThreshold", Range(-2,3)) = 1
		_EdgeWidth("EdgeWidth", Range(0,2)) = 1
		_RimPower("Rim Power", Range(-2,20)) = 1
		_RimColor("Rim Color Top", Color) = (0.5,0.5,0.5,1)
		_RimColor2("Rim Color Side/Bottom", Color) = (0.5,0.5,0.5,1)
		_SnowAmount("Snow Amount", Range(0, 1)) = 0
		_SnowTex("Snow Texture", 2D) = "white" {}

		_GridColorX ("Grid Color X", Color) = (0,0,0,1)
        _GridColorY ("Grid Color Y", Color) = (0,0,0,1)
        _GridColorSpecial ("Grid Color Special Highlight", Color) = (0,0,0,1)

		// ramp
        _ToonSteps ("Steps of Toon", range(1, 9)) = 2
        _RampThreshold ("Ramp Threshold", Range(0.1, 1)) = 0.5
        _RampSmooth ("Ramp Smooth", Range(0, 1)) = 0.1

		_HColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1.0)
        _SColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1.0)
    }
		
	
    CGINCLUDE
        #pragma surface surf Toon vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer 
        #pragma multi_compile_fog
		//#pragma multi_compile_instancing
        #include "CustomToonTerrain.cginc"

		float4 _GridColorX;
        float4 _GridColorY;
        float4 _GridColorSpecial;

        uniform float _GridOpacity;
		uniform float3 _GridPositionA;
        uniform float3 _GridPositionB;

		float _RampThreshold;
        float _RampSmooth;
        float _ToonSteps;

		fixed4 _HColor;
        fixed4 _SColor;

		

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 splat_control;
            half weight;
            fixed4 mixedDiffuse;

			// clamp (saturate) and increase(pow) the worldnormal value to use as a blend between the projected textures
			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));

			// normal noise triplanar for x, y, z sides
			float3 xn = tex2D(_Normal, IN.worldPos.zy * _NoiseScale);
			float3 yn = tex2D(_Normal, IN.worldPos.zx * _NoiseScale);
			float3 zn = tex2D(_Normal, IN.worldPos.xy * _NoiseScale);

			// lerped together all sides for noise texture
			float3 noisetexture = zn;
			noisetexture = lerp(noisetexture, xn, blendNormal.x);
			noisetexture = lerp(noisetexture, yn, blendNormal.y);

			// triplanar for top texture for x, y, z sides
			float3 xm = tex2D(_MainTex, IN.worldPos.zy * _Scale);
			float3 zm = tex2D(_MainTex, IN.worldPos.xy * _Scale);
			float3 ym = tex2D(_MainTex, IN.worldPos.zx * _Scale);

			// lerped together all sides for top texture
			float3 toptexture = zm;
			toptexture = lerp(toptexture, xm, blendNormal.x);
			toptexture = lerp(toptexture, ym, blendNormal.y);

			// triplanar for side and bottom texture, x,y,z sides
			float3 x = tex2D(_MainTexSide, IN.worldPos.zy * _SideScale);
			float3 y = tex2D(_MainTexSide, IN.worldPos.zx * _SideScale);
			float3 z = tex2D(_MainTexSide, IN.worldPos.xy * _SideScale);

			// lerped together all sides for side bottom texture
			float3 sidetexture = z;
			sidetexture = lerp(sidetexture, x, blendNormal.x);
			sidetexture = lerp(sidetexture, y, blendNormal.y);

			// rim light for fuzzy top texture
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal * noisetexture));

			// rim light for side/bottom texture
			half rim2 = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

			// dot product of world normal and surface normal + noise
			float worldNormalDotNoise = dot(o.Normal + (noisetexture.y + (noisetexture * 0.5)), IN.worldNormal.y);

			// if dot product is higher than the top spread slider, multiplied by triplanar mapped top texture
			// step is replacing an if statement to avoid branching :
			// if (worldNormalDotNoise > _TopSpread{ o.Albedo = toptexture}


			float3 topTextureResult = step(_TopSpread, worldNormalDotNoise) * toptexture;

			// if dot product is lower than the top spread slider, multiplied by triplanar mapped side/bottom texture
			float3 sideTextureResult = step(worldNormalDotNoise, _TopSpread) * sidetexture;

			// if dot product is in between the two, make the texture darker
			float3 topTextureEdgeResult = step(_TopSpread, worldNormalDotNoise) * step(worldNormalDotNoise, _TopSpread + _EdgeWidth) * -0.15;

			
            SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);

			float _step = step(dot(IN.localNormal, fixed3(0, 1, 0)), _TopThreshold);
			float3 _diffuse = mixedDiffuse.rgb*(1-_step);


		





			float _diffuseSide = topTextureResult + sideTextureResult + topTextureEdgeResult;
			_diffuseSide = _diffuseSide*(_step)*_Color;

			o.Albedo = _diffuse + _diffuseSide;
			o.Alpha = weight;
			o.Emission = step(_TopSpread, worldNormalDotNoise) * _RimColor.rgb * pow(rim, _RimPower) + step(worldNormalDotNoise, _TopSpread) * _RimColor2.rgb * pow(rim2, _RimPower);



			/*
			float _step = step(dot(IN.localNormal, fixed3(0, 1, 0)), _TopThreshold);
			float3 _diffuse = mixedDiffuse.rgb*_step*_TopColor; //0 if not greater

			float _diffuseSide = topTextureResult + sideTextureResult + topTextureEdgeResult;
			_diffuseSide = _diffuseSide*(1-_step)*_Color;

			o.Albedo = _diffuseSide ;
			o.Emission = step(_TopSpread, worldNormalDotNoise) * _RimColor.rgb * pow(rim, _RimPower) + step(worldNormalDotNoise, _TopSpread) * _RimColor2.rgb * pow(rim2, _RimPower);
			*/
			/*
			if (dot(IN.localNormal, fixed3(0, 1, 0)) >= _TopThreshold)
			{
				// snow
				// 
				//float3 snowColor = tex2D(_SnowTex, IN.tc_Control*100);
				//float l = mixedDiffuse.r * 0.3 + mixedDiffuse.g * 0.59 + mixedDiffuse.b * 0.11;
				//float factor = 1 - l;
				//snowColor *= (factor*1.1);

				//o.Albedo = mixedDiffuse.rgb + (_SnowAmount*snowColor);
				float4 color = lerp( mixedDiffuse , (1,1,1,1) , ( _SnowAmount ));
				o.Albedo = color.rgb;
			}
			else 
			{
				// final albedo color
				o.Albedo = topTextureResult + sideTextureResult + topTextureEdgeResult;
				o.Albedo *= _Color;

				// adding the fuzzy rimlight(rim) on the top texture, and the harder rimlight (rim2) on the side/bottom texture
				o.Emission = step(_TopSpread, worldNormalDotNoise) * _RimColor.rgb * pow(rim, _RimPower) + step(worldNormalDotNoise, _TopSpread) * _RimColor2.rgb * pow(rim2, _RimPower);
			}
			
            o.Alpha = weight;
			*/
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float3 color = o.Albedo;
			
			float3 position = IN.worldPos;
            float3 positionRaw = position;

			float stepSpecial = step(_GridPositionA.x, positionRaw.x);
            stepSpecial = step(positionRaw.x, _GridPositionB.x)*stepSpecial;
            stepSpecial = step(_GridPositionA.z, positionRaw.z)*stepSpecial;
            stepSpecial = step(positionRaw.z, _GridPositionB.z)*stepSpecial;

			float world_dot = step(_TopSpread, worldNormalDotNoise);

			position = position % 1;
            _step = step(0.1, position.x);
            _step = saturate(_step + (1-_GridOpacity));
            float inverse = 1 - _step;
            float inverseSum = inverse;
			inverse = inverse*world_dot;
            color = color*(1-inverse) + _GridColorX*inverse;
            _step = step(0.1, position.z);
            _step = saturate(_step + (1-_GridOpacity));
            inverse = 1 - _step;
			inverse = inverse*world_dot;
            inverseSum += inverse;
            color = color*(1-inverse) + _GridColorY*inverse;

			//inverseSum = saturate(inverseSum);
            //stepSpecial *= inverseSum;
            color = color - _GridColorSpecial*stepSpecial;
			//color = color*step(_TopSpread, worldNormalDotNoise); 

/*
            _step = step(0.1, position.x);
            _step = saturate(_step + (1-_GridOpacity));
            color *= (_step);
            float inverse = 1 - _step;
            color += _GridColorX*inverse;


            _step = step(0.1, position.z);
            _step = saturate(_step + (1-_GridOpacity));
            color *= (_step);
            inverse = 1 - _step;
            color += _GridColorY*inverse;

*/
            o.Albedo = color;
            //o.Alpha = c.a;
        }
    ENDCG



    Category {
        Tags {
            "Queue" = "Geometry-99"
            "RenderType" = "Opaque"
			"TerrainCompatible" = "True"
        }
		
        // TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        
		SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile __ _TERRAIN_NORMAL_MAP
				#pragma multi_compile_instancing
				
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
					float3 rampColor = lerp(_SColor.rgb, _Color.rgb, ramp);
					
					
					fixed3 lightColor = _LightColor0.rgb;
					
					fixed4 color;
					fixed3 diffuse = s.Albedo * lightColor * rampColor;

					
					color.rgb = diffuse ;
					color.a = s.Alpha;
					return color;
				}
            ENDCG

			
        }
        SubShader { // for sm2.0 targets
            CGPROGRAM

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
				float3 rampColor = lerp(_SColor.rgb, _Color.rgb, ramp);
				
				
				fixed3 lightColor = _LightColor0.rgb;
				
				fixed4 color;
				fixed3 diffuse = s.Albedo * lightColor * rampColor;

				
				color.rgb = diffuse ;
				color.a = s.Alpha;
				return color;
			}
            ENDCG
        }
		
    }

    Dependency "BaseMapShader" = "Diffuse"
    Dependency "Details0"      = "Hidden/TerrainEngine/Details/Vertexlit"
    Dependency "Details1"      = "Hidden/TerrainEngine/Details/WavingDoublePass"
    Dependency "Details2"      = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
    Dependency "Tree0"         = "Hidden/TerrainEngine/BillboardTree"

    Fallback "Diffuse"
}
