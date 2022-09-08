// Upgrade NOTE: removed variant '__' where variant LOD_FADE_PERCENTAGE is used.

Shader "CTI/LOD Leaves" {
Properties {

	[Space(5)]
	[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0
	
	[Space(5)]
	_HueVariation						("Color Variation", Color) = (0.9,0.5,0.0,0.1)
	[Space(5)]
	_MainTex 							("Albedo (RGB) Alpha (A)", 2D) = "white" {}
	_Cutoff 							("Alpha Cutoff", Range(0,1)) = 0.3
	
	[Space(5)]
	[NoScaleOffset] _BumpSpecMap 		("Normal Map (GA) Specular (B)", 2D) = "bump" {}
	[NoScaleOffset] _TranslucencyMap 	("AO (G) Translucency (B) Smoothness (A)", 2D) = "white" {}

	[Space(5)]
	_TranslucencyStrength				("Translucency Strength", Range(0,1)) = 0.5
	_ViewDependency						("View Dependency", Range(0,1)) = 0.8
	[Toggle(_PARALLAXMAP)] _EnableTransFade("Fade out Translucency", Float) = 0.0

	[Header(Wind)]
	[Space(3)]
	_TumbleStrength						("Tumble Strength", Range(-1,1)) = 0
	_TumbleFrequency					("Tumble Frequency", Range(0,4)) = 1
	_TimeOffset							("Time Offset", Range(0,2)) = 0.25
	[Space(3)]
	[Toggle(_EMISSION)] _EnableLeafTurbulence("Enable Leaf Turbulence", Float) = 0.0
	_LeafTurbulence 					("Leaf Turbulence", Range(0,4)) = 0.2
	_EdgeFlutterInfluence				("Edge Flutter Influence", Range(0,1)) = 0.25
	[Space(5)]
	[Toggle(_METALLICGLOSSMAP)] _LODTerrain ("Use Wind from Script", Float) = 0.0
	[Space(10)]

	[Header(Animate Normals)]
	[Space(3)]
	[Toggle(_NORMALMAP)] _AnimateNormal	("Enable Normal Rotation", Float) = 0.0
	[Space(10)]

	[Header(Options for lowest LOD)]
	[Space(3)]
	[Toggle] _FadeOutWind				("Fade out Wind", Float) = 0.0

	// Needed by VegetationStudio's Billboard Rendertex Shaders
	[HideInInspector] _IsBark("Is Bark", Float) = 0
}

SubShader { 
	Tags {

		"Queue"="Geometry+1"		
		"IgnoreProjector"="True"
		"RenderType"="CTI-TreeLeafLOD"
		"DisableBatching" = "LODFading"
	}
	LOD 200
	Cull [_Culling]

	CGPROGRAM

// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does	
		#pragma surface surf StandardTranslucent vertex:CTI_TreeVertLeaf fullforwardshadows dithercrossfade
// alphatest:_Cutoff 
// nolightmap
		#pragma target 3.0
		#pragma multi_compile  LOD_FADE_CROSSFADE LOD_FADE_PERCENTAGE

		// Use Wind from Script
		#pragma shader_feature _METALLICGLOSSMAP
		// LeafTurbulence
		#pragma shader_feature _EMISSION
		// AnimateNormal
		#pragma shader_feature _NORMALMAP
		// Fade out Translucency
		#pragma shader_feature _PARALLAXMAP
		
		#pragma multi_compile_instancing

	//	#if UNITY_VERSION >= 550
			#pragma instancing_options assumeuniformscaling lodfade procedural:setup 	
	//	#endif

		// #include "UnityBuiltin3xTreeLibrary.cginc" // We can not do this as we want instancing
		#define USE_VFACE
		#define LEAFTUMBLING
		#define IS_LODTREE
		#define IS_SURFACESHADER
		
		#include "Includes/CTI_TranslucentLighting.cginc"
		#include "Includes/CTI_Builtin4xTreeLibraryTumbling.cginc"
		#include "Includes/CTI_indirect.cginc"	

		sampler2D _MainTex;
		sampler2D _BumpSpecMap;
		sampler2D _TranslucencyMap;
		half _TranslucencyStrength;
		half _ViewDependency;
		half _Cutoff;

		/* moved to include
		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR; // color.a = AO
			UNITY_DITHER_CROSSFADE_COORDS
		};
		*/

		void surf (Input IN, inout SurfaceOutputStandardTranslucent o) {

			#if UNITY_VERSION < 2017
    			UNITY_APPLY_DITHER_CROSSFADE(IN)
   			#endif

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			clip (c.a - _Cutoff);
			o.Alpha = c.a;
			
		//	Add Color Variation
			o.Albedo = lerp(c.rgb, (c.rgb + _HueVariation.rgb) * 0.5, IN.color.r * _HueVariation.a);
			fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
			
			o.Translucency = trngls.b * _TranslucencyStrength
		//	Fade out translucency	
			#if defined(_PARALLAXMAP)
				* IN.color.b
			#endif
			;

			o.ScatteringPower = _ViewDependency;
		//	Fade out smoothness
			o.Smoothness = trngls.a
			#if defined(_PARALLAXMAP)
				* IN.color.b
			#endif
			;

			o.Occlusion = trngls.g * IN.color.a;
				
			half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
			o.Specular = norspc.b;
			o.Normal = UnpackNormalDXT5nm(norspc) * float3(1,1,IN.FacingSign);
		}
	ENDCG

	// Pass to render object as a shadow caster
	// Do not forget to setup the instance ID!
	Pass {
		Name "ShadowCaster"
		Tags { 
			"LightMode" = "ShadowCaster"
		}
		
		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma target 3.0
		#pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma multi_compile_instancing
		//#if UNITY_VERSION >= 550
			#pragma instancing_options assumeuniformscaling lodfade procedural:setup
		//#endif

		// Use Wind from Script
		#pragma shader_feature _METALLICGLOSSMAP
		// LeafTurbulence
		#pragma shader_feature _EMISSION

		#pragma multi_compile_shadowcaster
		
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define UNITY_PASS_SHADOWCASTER
		// #include "UnityBuiltin3xTreeLibrary.cginc" // We can not do this as we want instancing
		#define USE_VFACE
		#define LEAFTUMBLING
		#define DEPTH_NORMAL
		#define IS_LODTREE
		#include "Includes/CTI_Builtin4xTreeLibraryTumbling.cginc"
		#include "Includes/CTI_indirect.cginc"	

		sampler2D _MainTex;

	//  Already defined in include
	//	struct Input {
	//		float2 uv_MainTex;
	//	};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
			UNITY_DITHER_CROSSFADE_COORDS_IDX(2)
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
		};

		float4 _MainTex_ST;
		
		v2f_surf vert_surf (appdata_ctitree v) {
			v2f_surf o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			CTI_TreeVertLeaf(v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)
			return o;
		}
		fixed _Cutoff;
		
		float4 frag_surf (v2f_surf IN) : SV_Target {
			UNITY_SETUP_INSTANCE_ID(IN);
			#if UNITY_VERSION < 2017
				UNITY_APPLY_DITHER_CROSSFADE(IN)
			#else
				UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
			#endif
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}


///
}

CustomEditor "CTI_ShaderGUI"

}
