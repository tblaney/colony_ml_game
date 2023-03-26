// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Toon/Terrain/First-Snow" {
    Properties {
        _Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Top Texture", 2D) = "white" {}
		_MainTexSide("Side/Bottom Texture", 2D) = "white" {}
		_Scale("Top Scale", Range(-2,2)) = 1
		_SideScale("Side Scale", Range(-2,2)) = 1
		_TopSpread("TopSpread", Range(-2,8)) = 1
		_TopThreshold("TopThreshold", Range(-2,3)) = 1
		_EdgeWidth("EdgeWidth", Range(0,2)) = 1

		_GridColorX ("Grid Color X", Color) = (0,0,0,1)
        _GridColorY ("Grid Color Y", Color) = (0,0,0,1)

        _ToonSteps ("Steps of Toon", range(1, 9)) = 2
        _RampThreshold ("Ramp Threshold", Range(0.1, 1)) = 0.5
        _RampSmooth ("Ramp Smooth", Range(0, 1)) = 0.1

		_HColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1.0)
        _SColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1.0)
    }

    CGINCLUDE
        #pragma surface surf Toon vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer addshadow fullforwardshadows
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        #pragma multi_compile_fog
        #include "TraegisTerrain.cginc"


        float4 _GridColorX;
        float4 _GridColorY;

        uniform float _GridOpacity;

		float _RampThreshold;
        float _RampSmooth;
        float _ToonSteps;

		fixed4 _HColor;
        fixed4 _SColor;

        sampler2D _MainTex, _MainTexSide;
        float4 _Color;
        float _RimPower;
        float  _TopSpread, _EdgeWidth;
        float _TopThreshold;
        float _Scale, _SideScale;
        uniform float4 _MainColor;

        void surf(Input IN, inout SurfaceOutput o)
        {
            /*
			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));

            float3 x = tex2D(_MainTexSide, IN.worldPos.zy * _SideScale);
			float3 y = tex2D(_MainTexSide, IN.worldPos.zx * _SideScale);
			float3 z = tex2D(_MainTexSide, IN.worldPos.xy * _SideScale);
            float3 sidetexture = z;
			sidetexture = lerp(sidetexture, x, blendNormal.x);
			sidetexture = lerp(sidetexture, y, blendNormal.y);

			float world_normal = dot(o.Normal, IN.worldNormal.y);
            half4 splat_control;
            half weight;
            fixed4 mixedDiffuse;
            SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);

            float3 _albedo = mixedDiffuse.rgb*(step(_TopSpread, world_normal))*_MainColor + sidetexture*(step(world_normal, _TopSpread));
            float3 _alpha = weight;

            float3 color = _albedo;
			float3 position = IN.worldPos;
            float3 positionRaw = position;

			float world_dot = step(_TopSpread, world_normal);
			position = position % 1;
            float _step = step(0.05, position.x);
            _step = saturate(_step + (1-_GridOpacity));
            float inverse = 1 - _step;
            float inverseSum = inverse;
			inverse = inverse*world_dot;
            color = color*(1-inverse) + _GridColorX*inverse;
            _step = step(0.05, position.z);
            _step = saturate(_step + (1-_GridOpacity));
            inverse = 1 - _step;
			inverse = inverse*world_dot;
            inverseSum += inverse;
            color = color*(1-inverse) + _GridColorY*inverse;

            o.Albedo = color;
            o.Alpha = 1;

            */

            half4 splat_control;
            half weight;
            fixed4 mixedDiffuse;

			// clamp (saturate) and increase(pow) the worldnormal value to use as a blend between the projected textures
			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));

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


			// dot product of world normal and surface normal + noise
			float worldNormalDotNoise = dot(o.Normal, IN.worldNormal.y);

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

            float3 color = o.Albedo;

			float3 position = IN.worldPos;
            position = position % 1;

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


            o.Albedo = color;
        }
    ENDCG

    Category {
        Tags {
            "Queue" = "Geometry-99"
            "RenderType" = "Opaque"
			"TerrainCompatible" = "True"
        }

        // TODO: Seems like "#pragma target 3.0 _NORMALMAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile_local __ _ALPHATEST_ON
                #pragma multi_compile_local __ _NORMALMAP

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

                    color.rgb = diffuse;
                    color.a = s.Alpha;
                    return color;
                }
            ENDCG

            UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
            UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
        }
        SubShader 
        { // for sm2.0 targets
        
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

                    
                    color.rgb = diffuse;
                    color.a = s.Alpha;
                    return color;
                }
            ENDCG
        }
    }

    Dependency "AddPassShader"    = "Hidden/TerrainEngine/Splatmap/Diffuse-AddPass"
    Dependency "BaseMapShader"    = "Hidden/TerrainEngine/Splatmap/Diffuse-Base"
    Dependency "BaseMapGenShader" = "Hidden/TerrainEngine/Splatmap/Diffuse-BaseGen"
    Dependency "Details0"         = "Hidden/TerrainEngine/Details/Vertexlit"
    Dependency "Details1"         = "Hidden/TerrainEngine/Details/WavingDoublePass"
    Dependency "Details2"         = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
    Dependency "Tree0"            = "Hidden/TerrainEngine/BillboardTree"

    Fallback "Diffuse"
}
