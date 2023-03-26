// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Edits by Glynn Taylor. MIT license
// Includes code for splitmap by https://twitter.com/adamgryu and triplanar mapping by https://github.com/keijiro. MIT License

Shader "CustomToonTerrain" {
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
        //_GridOpacity ("Grid Opacity", Range(0, 1)) = 1
    }
		
	
    CGINCLUDE
        #pragma surface surf Lambert vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma multi_compile_fog
        #include "CustomToonTerrain.cginc"

		float4 _GridColorX;
        float4 _GridColorY;
        uniform float _GridOpacity;

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
        // TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        
		SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile __ _TERRAIN_NORMAL_MAP
            ENDCG

			
        }
        SubShader { // for sm2.0 targets
            CGPROGRAM
            ENDCG
        }
		
    }

    Dependency "AddPassShader" = "Hidden/CustomToonTerrainAdd"
    Dependency "BaseMapShader" = "Diffuse"
    Dependency "Details0"      = "Hidden/TerrainEngine/Details/Vertexlit"
    Dependency "Details1"      = "Hidden/TerrainEngine/Details/WavingDoublePass"
    Dependency "Details2"      = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
    Dependency "Tree0"         = "Hidden/TerrainEngine/BillboardTree"

    Fallback "Diffuse"
}
