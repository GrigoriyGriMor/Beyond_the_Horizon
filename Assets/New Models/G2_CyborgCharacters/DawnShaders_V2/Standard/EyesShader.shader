// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DawnShader_V2/Eyes"
{
	Properties
	{
		[HideInInspector] _VTInfoBlock( "VT( auto )", Vector ) = ( 0, 0, 0, 0 )
		_BC("Base Color", 2D) = "white" {}
		_Irissize("Iris size", Range( 0 , 2)) = 0.93
		_IrisBrightnessPower("Iris Brightness Power", Float) = 1
		_Roughness("Roughness", 2D) = "white" {}
		_RoughnessBoost("Roughness Boost", Range( 0 , 1)) = 1
		_Coat("Coat", 2D) = "white" {}
		_CoatBoost("Coat Boost", Range( 0 , 1)) = 1
		_EyeMask("Eye Mask", 2D) = "white" {}
		_IrisColor("Iris Color", Color) = (1,1,1,0)
		_IrisColorBoost("Iris Color Boost", Range( 0 , 1)) = 0
		_ScleraColor("Sclera Color", Color) = (1,1,1,0)
		_ScleraColorBoost("Sclera Color Boost", Range( 0 , 1)) = 0
		_EmissiveIrisColor("Emissive Iris Color", Color) = (0,0,0,0)
		_EmissiveIrisPower("Emissive Iris Power ", Range( 0 , 2000)) = 0
		_EmissiveIrisInnerColor("Emissive Iris Inner Color", Color) = (0,0,0,0)
		_EmissiveIrisInnerPower("Emissive Iris Inner Power ", Range( 0 , 2000)) = 0
		_EmissiveScleraColor("Emissive Sclera Color ", Color) = (0,0,0,0)
		_EmissiveScleraPower("Emissive Sclera Power", Range( 0 , 2000)) = 0
		_EyePOMmask("Eye POM mask", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 0.5)) = 0.1
		_CurvatureV("Curvature V", Range( 0 , 100)) = 0
		_CurvatureU("Curvature U", Range( 0 , 100)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[Header(Parallax Occlusion Mapping)]
		_CurvFix("Curvature Bias", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "Amplify" = "True"  "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform sampler2D _BC;
		uniform float _Irissize;
		uniform sampler2D _EyePOMmask;
		uniform float _Scale;
		uniform float _CurvFix;
		uniform float _CurvatureU;
		uniform float _CurvatureV;
		uniform float4 _EyePOMmask_ST;
		uniform float _IrisBrightnessPower;
		uniform sampler2D _EyeMask;
		uniform float4 _ScleraColor;
		uniform float4 _IrisColor;
		uniform float _IrisColorBoost;
		uniform float _ScleraColorBoost;
		uniform float4 _EmissiveIrisColor;
		uniform float _EmissiveIrisPower;
		uniform float4 _EmissiveScleraColor;
		uniform float _EmissiveScleraPower;
		uniform float4 _EmissiveIrisInnerColor;
		uniform float _EmissiveIrisInnerPower;
		uniform float _CoatBoost;
		uniform sampler2D _Coat;
		uniform float4 _Coat_ST;
		uniform float _RoughnessBoost;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs.xy += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
			 	result.z = dot( curv, currTexOffset * currTexOffset );
			 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r * ( 1 - result.z );
			 	if ( currHeight > currRayZ )
			 	{
			 	 	stepIndex = numSteps + 1;
			 	}
			 	else
			 	{
			 	 	stepIndex++;
			 	 	prevTexOffset = currTexOffset;
			 	 	prevRayZ = currRayZ;
			 	 	prevHeight = currHeight;
			 	 	currTexOffset += deltaTex;
			 	 	currRayZ -= layerHeight * ( 1 - result.z ) * (1+_CurvFix);
			 	}
			}
			int sectionSteps = 2;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
			 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
			 	finalTexOffset = prevTexOffset + intersection * deltaTex;
			 	newZ = prevRayZ - intersection * layerHeight;
			 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
			 	if ( newHeight > newZ )
			 	{
			 	 	currTexOffset = finalTexOffset;
			 	 	currHeight = newHeight;
			 	 	currRayZ = newZ;
			 	 	deltaTex = intersection * deltaTex;
			 	 	layerHeight = intersection * layerHeight;
			 	}
			 	else
			 	{
			 	 	prevTexOffset = finalTexOffset;
			 	 	prevHeight = newHeight;
			 	 	prevRayZ = newZ;
			 	 	deltaTex = ( 1 - intersection ) * deltaTex;
			 	 	layerHeight = ( 1 - intersection ) * layerHeight;
			 	}
			 	sectionIndex++;
			}
			#ifdef UNITY_PASS_SHADOWCASTER
			if ( unity_LightShadowBias.z == 0.0 )
			{
			#endif
			 	if ( result.z > 1 )
			 	 	clip( -1 );
			#ifdef UNITY_PASS_SHADOWCASTER
			}
			#endif
			return uvs.xy + finalTexOffset;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 temp_cast_0 = (_Irissize).xx;
			float2 temp_cast_1 = (( ( 1.0 - _Irissize ) / 2.0 )).xx;
			float2 uv_TexCoord204 = i.uv_texcoord * temp_cast_0 + temp_cast_1;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 appendResult201 = (float2(_CurvatureU , _CurvatureV));
			float2 OffsetPOM205 = POM( _EyePOMmask, uv_TexCoord204, ddx(uv_TexCoord204), ddy(uv_TexCoord204), ase_worldNormal, ase_worldViewDir, i.viewDir, 8, 8, _Scale, 0, _EyePOMmask_ST.xy, appendResult201, 0 );
			float2 EyePOM207 = OffsetPOM205;
			float4 tex2DNode1 = tex2D( _BC, EyePOM207 );
			float4 tex2DNode231 = tex2D( _EyeMask, EyePOM207 );
			float4 lerpResult211 = lerp( tex2DNode1 , ( tex2DNode1 * _IrisBrightnessPower ) , tex2DNode231.r);
			float4 lerpResult30 = lerp( float4( 0,0,0,0 ) , _ScleraColor , tex2DNode231.b);
			float4 lerpResult28 = lerp( float4( 0,0,0,0 ) , ( _IrisColor * tex2DNode1.r ) , tex2DNode231.r);
			float4 lerpResult132 = lerp( lerpResult211 , ( lerpResult30 + lerpResult28 ) , ( ( _IrisColorBoost * tex2DNode231.r ) + ( _ScleraColorBoost * tex2DNode231.b ) ));
			float4 BaseColor219 = lerpResult132;
			o.Albedo = BaseColor219.rgb;
			float4 Emission223 = ( ( tex2DNode231.r * _EmissiveIrisColor * _EmissiveIrisPower ) + ( tex2DNode231.b * _EmissiveScleraColor * _EmissiveScleraPower ) + ( tex2DNode231.g * _EmissiveIrisInnerColor * _EmissiveIrisInnerPower ) );
			o.Emission = Emission223.rgb;
			float2 uv_Coat = i.uv_texcoord * _Coat_ST.xy + _Coat_ST.zw;
			float4 Coat347 = ( _CoatBoost * ( 1.0 - tex2D( _Coat, uv_Coat ) ) );
			o.Metallic = Coat347.r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float4 Roughness225 = ( _RoughnessBoost * ( 1.0 - tex2D( _Roughness, uv_Roughness ) ) );
			o.Smoothness = Roughness225.r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
2560;0;2560;1059;-383.3838;2072.957;1.129481;True;False
Node;AmplifyShaderEditor.RangedFloatNode;227;-2956.647,-3896.87;Inherit;False;Property;_Irissize;Iris size;1;0;Create;True;0;0;0;False;0;False;0.93;0.965;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;228;-2585.611,-3779.067;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;199;-2397.459,-3269.843;Inherit;False;Property;_CurvatureU;Curvature U;21;0;Create;True;0;0;0;False;0;False;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;198;-2403.011,-3166.238;Inherit;False;Property;_CurvatureV;Curvature V;20;0;Create;True;0;0;0;False;0;False;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;229;-2431.382,-3777.335;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2053.456,-3255.943;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VirtualTextureObject;203;-2524.297,-3649.317;Inherit;True;Property;_EyePOMmask;Eye POM mask;18;0;Create;True;0;0;0;False;0;False;-1;8e0684ecbe323eb4d8877824df35abed;8e0684ecbe323eb4d8877824df35abed;False;white;Auto;Unity5;0;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;200;-2203.886,-3462.521;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;202;-2225.838,-3561.271;Inherit;False;Property;_Scale;Scale;19;0;Create;True;0;0;0;False;0;False;0.1;0.132;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;204;-2263.714,-3909.162;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;205;-1831.313,-3675.796;Inherit;False;0;8;False;-1;16;False;-1;2;0.02;0;False;1,1;True;0,0;8;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;7;SAMPLERSTATE;;False;2;FLOAT;0.02;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-1541.599,-3677.664;Inherit;False;EyePOM;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;-3084.647,-1968.914;Inherit;True;207;EyePOM;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-2714.104,-1994.34;Inherit;True;Property;_BC;Base Color;0;0;Create;False;0;0;0;False;0;False;-1;ecc914b2ef202e142b95489ec74f0cdd;ecc914b2ef202e142b95489ec74f0cdd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;230;-3046.157,-2355.804;Inherit;True;Property;_EyeMask;Eye Mask;7;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;8;-2168.097,-2201.844;Float;False;Property;_IrisColor;Iris Color;8;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.6089801,0.8018868,0.7434302,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;231;-2712.914,-2357.841;Inherit;True;Property;_TextureSample0;Texture Sample 0;28;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;232;-2034.829,-2336.213;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-2599.823,-1557.917;Inherit;False;Property;_IrisBrightnessPower;Iris Brightness Power;2;0;Create;True;0;0;0;False;0;False;1;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;189;-2434.851,-2863.941;Inherit;False;Property;_ScleraColorBoost;Sclera Color Boost;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-1923.269,-2205.002;Float;False;Property;_ScleraColor;Sclera Color;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;192;-2432.624,-2948.33;Inherit;False;Property;_IrisColorBoost;Iris Color Boost;9;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;344;-2632.879,-439.3299;Inherit;True;Property;_Coat;Coat;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;135;-2065,-1349;Float;False;Property;_EmissiveIrisPower;Emissive Iris Power ;13;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;77;-2603.537,53.28463;Inherit;True;Property;_Roughness;Roughness;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;134;-2033.259,-1535.906;Float;False;Property;_EmissiveIrisColor;Emissive Iris Color;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;140;-2076.311,-1054.009;Float;False;Property;_EmissiveScleraPower;Emissive Sclera Power;17;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;139;-2030.311,-1255.009;Float;False;Property;_EmissiveScleraColor;Emissive Sclera Color ;16;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-2097.571,-2849.496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;351;-2074.975,-771.9985;Float;False;Property;_EmissiveIrisInnerPower;Emissive Iris Inner Power ;15;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-2325.555,-1651.231;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;28;-1892.17,-2532.302;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;350;-2043.234,-958.9044;Float;False;Property;_EmissiveIrisInnerColor;Emissive Iris Inner Color;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-2096.399,-2950.01;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;-1613.953,-2545.066;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-1387.284,-2345.181;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;343;-2622.129,-569.9596;Float;False;Property;_CoatBoost;Coat Boost;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;211;-1537.929,-1962.182;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-1655.342,-1275.811;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;345;-2261.489,-436.635;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;352;-1653.006,-977.8004;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;212;-2232.147,55.97946;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1669.031,-1548.802;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;197;-1467.484,-2949.041;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-2476.893,-226.8009;Float;False;Property;_RoughnessBoost;Roughness Boost;4;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-2057.737,-52.98496;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;346;-2060.08,-514.5994;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;-1219.227,-1208.737;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;132;-1010.112,-2333.926;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;347;-1797.006,-502.4946;Inherit;False;Coat;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-1079.249,-1217.632;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;225;-1809.664,-174.8801;Inherit;False;Roughness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;219;-784.8649,-2342.487;Inherit;False;BaseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;226;1153.757,-1281.641;Inherit;False;225;Roughness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;220;1156.763,-1545.808;Inherit;False;219;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;348;1153.666,-1449.927;Inherit;False;347;Coat;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;1154.094,-1361.795;Inherit;False;223;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;349;1443.579,-1523.725;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;DawnShader_V2/Eyes;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;228;0;227;0
WireConnection;229;0;228;0
WireConnection;201;0;199;0
WireConnection;201;1;198;0
WireConnection;204;0;227;0
WireConnection;204;1;229;0
WireConnection;205;0;204;0
WireConnection;205;1;203;0
WireConnection;205;2;202;0
WireConnection;205;3;200;0
WireConnection;205;5;201;0
WireConnection;207;0;205;0
WireConnection;1;1;218;0
WireConnection;231;0;230;0
WireConnection;231;1;218;0
WireConnection;232;0;8;0
WireConnection;232;1;1;1
WireConnection;195;0;189;0
WireConnection;195;1;231;3
WireConnection;210;0;1;0
WireConnection;210;1;209;0
WireConnection;28;1;232;0
WireConnection;28;2;231;1
WireConnection;196;0;192;0
WireConnection;196;1;231;1
WireConnection;30;1;13;0
WireConnection;30;2;231;3
WireConnection;188;0;30;0
WireConnection;188;1;28;0
WireConnection;211;0;1;0
WireConnection;211;1;210;0
WireConnection;211;2;231;1
WireConnection;141;0;231;3
WireConnection;141;1;139;0
WireConnection;141;2;140;0
WireConnection;345;0;344;0
WireConnection;352;0;231;2
WireConnection;352;1;350;0
WireConnection;352;2;351;0
WireConnection;212;0;77;0
WireConnection;138;0;231;1
WireConnection;138;1;134;0
WireConnection;138;2;135;0
WireConnection;197;0;196;0
WireConnection;197;1;195;0
WireConnection;117;0;79;0
WireConnection;117;1;212;0
WireConnection;346;0;343;0
WireConnection;346;1;345;0
WireConnection;154;0;138;0
WireConnection;154;1;141;0
WireConnection;154;2;352;0
WireConnection;132;0;211;0
WireConnection;132;1;188;0
WireConnection;132;2;197;0
WireConnection;347;0;346;0
WireConnection;223;0;154;0
WireConnection;225;0;117;0
WireConnection;219;0;132;0
WireConnection;349;0;220;0
WireConnection;349;2;224;0
WireConnection;349;3;348;0
WireConnection;349;4;226;0
ASEEND*/
//CHKSM=6E674C143C4EAD0384E456B81DB7545E9F512D66