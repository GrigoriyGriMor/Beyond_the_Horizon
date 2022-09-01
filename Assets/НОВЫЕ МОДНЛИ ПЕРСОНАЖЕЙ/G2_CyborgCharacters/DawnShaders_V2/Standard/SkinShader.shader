// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DawnShader_V2/Skin"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_TattoMask("Tatto Mask", 2D) = "white" {}
		_TattooColor("Tattoo Color", Color) = (0.1422659,0.1889629,0.2169811,0)
		_TattooBoots("Tattoo Boots", Range( 0 , 1)) = 0
		_Dirt("Dirt", 2D) = "white" {}
		_DirtColor("Dirt Color", Color) = (0.1509434,0.0874304,0.05339979,0)
		_DirtBoots("Dirt Boots", Range( 0 , 1)) = 0
		_DirtRoughness_Power("DirtRoughness_Power", Float) = 0
		_SkinNormal("Skin Normal", 2D) = "bump" {}
		_SkinColor("Skin Color", 2D) = "white" {}
		_ORM("ORM", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_SSSColor("SSS Color", Color) = (1,0.7457517,0.6839622,0)
		[Toggle(_ISCYBORG_ON)] _isCyborg("is Cyborg", Float) = 0
		_AMS("AMS", 2D) = "white" {}
		_MetallicMultiply("Metallic Multiply", Range( 0 , 1)) = 0.35
		_SkinRounghnessBoots("Skin Rounghness Boots", Range( 0 , 2)) = 1
		_RoughnessMod_R("RoughnessMod_R", 2D) = "white" {}
		_SkinPore("Skin Pore", 2D) = "bump" {}
		_SkinPore_Tilling("SkinPore_Tilling", Float) = 35
		_SkinPore_Power("SkinPore_Power", Range( 0 , 1)) = 0.25
		_BaseColorMul("BaseColorMul", Color) = (1,1,1,0)
		_ColorPow("Color Pow", Range( 0 , 10)) = 0
		_LipNailColor("Lip/Nail Color", Color) = (1,1,1,0)
		_LipNailColorPower("Lip/Nail Color Power", Range( 0 , 1)) = 0
		_LipsNailRoughnessBoost("Lips/Nail Roughness Boost", Range( 0 , 1)) = 0.75
		_FreaklesColor("Freakles Color", Color) = (0,0,0,0)
		_FreaklesColorPower("Freakles Color Power", Range( 0 , 20)) = 0
		_HairCapColor("HairCap Color", Color) = (0,0,0,0)
		_HairCapColorPower("HairCap Color Power", Range( 0 , 5)) = 0
		_HairCapRoughness("HairCap Roughness", Range( 0 , 1)) = 0.6
		[Toggle(_ISEXTRAMASKEUP_ON)] _IsExtraMaskeUp("Is Extra MaskeUp", Float) = 0
		_ExtraMask("Extra Mask", 2D) = "white" {}
		_EyeShadowOutColor("Eye Shadow Out  Color", Color) = (1,1,1,0)
		_EyeShadowOutPower("Eye Shadow Out Power", Range( 0 , 1)) = 0
		_EyeShadowInColor("Eye Shadow In  Color", Color) = (1,1,1,0)
		_EyeShadowInPower("Eye Shadow In Power", Range( 0 , 1)) = 0
		_BlushColor("Blush Color", Color) = (1,1,1,0)
		_BlushPower("Blush Power", Range( 0 , 1)) = 0
		_EmissiveMask("Emissive Mask", 2D) = "white" {}
		_Emissivepowerboost("Emissive power boost", Float) = 100
		_EmissiveColor1("Emissive Color 1", Color) = (0,0,0,0)
		_EmissivePower1("Emissive Power 1", Range( 0 , 2000)) = 0
		_EmissiveColor2("Emissive Color 2", Color) = (0,0,0,0)
		_EmissivePower2("Emissive Power 2", Range( 0 , 2000)) = 0
		_EmissiveColor3("Emissive Color 3", Color) = (0,0,0,0)
		_EmissivePower3("Emissive Power 3", Range( 0 , 2000)) = 0
		_EmissivePannerMap("Emissive Panner Map", 2D) = "white" {}
		_EmissivePannerTilling("Emissive Panner Tilling", Float) = 1
		_PannerProperty("Panner Property", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input {
			half filler;
		};
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISCYBORG_ON
		#pragma shader_feature_local _ISEXTRAMASKEUP_ON
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Transmission;
		};

		uniform sampler2D _SkinNormal;
		uniform float4 _SkinNormal_ST;
		uniform sampler2D _SkinPore;
		uniform float _SkinPore_Tilling;
		uniform float _SkinPore_Power;
		uniform sampler2D _AMS;
		uniform float4 _AMS_ST;
		uniform float4 _BaseColorMul;
		uniform float _ColorPow;
		uniform sampler2D _SkinColor;
		uniform float4 _SkinColor_ST;
		uniform float4 _LipNailColor;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float4 _FreaklesColor;
		uniform float4 _HairCapColor;
		uniform float _LipNailColorPower;
		uniform float _FreaklesColorPower;
		uniform float _HairCapColorPower;
		uniform float4 _EyeShadowOutColor;
		uniform sampler2D _ExtraMask;
		uniform float4 _ExtraMask_ST;
		uniform float4 _EyeShadowInColor;
		uniform float4 _BlushColor;
		uniform float _EyeShadowOutPower;
		uniform float _EyeShadowInPower;
		uniform float _BlushPower;
		uniform float4 _TattooColor;
		uniform sampler2D _TattoMask;
		uniform float4 _TattoMask_ST;
		uniform float _TattooBoots;
		uniform float _MetallicMultiply;
		uniform float _DirtBoots;
		uniform float4 _DirtColor;
		uniform sampler2D _Dirt;
		uniform float4 _Dirt_ST;
		uniform float _Emissivepowerboost;
		uniform float4 _EmissiveColor1;
		uniform float _EmissivePower1;
		uniform sampler2D _EmissiveMask;
		uniform float4 _EmissiveMask_ST;
		uniform sampler2D _EmissivePannerMap;
		uniform float2 _PannerProperty;
		uniform float _EmissivePannerTilling;
		uniform float4 _EmissiveColor2;
		uniform float _EmissivePower2;
		uniform float4 _EmissiveColor3;
		uniform float _EmissivePower3;
		uniform sampler2D _ORM;
		uniform float4 _ORM_ST;
		uniform float _SkinRounghnessBoots;
		uniform sampler2D _RoughnessMod_R;
		uniform float4 _RoughnessMod_R_ST;
		uniform float _DirtRoughness_Power;
		uniform float _HairCapRoughness;
		uniform float _LipsNailRoughnessBoost;
		uniform float4 _SSSColor;

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_SkinNormal = i.uv_texcoord * _SkinNormal_ST.xy + _SkinNormal_ST.zw;
			float3 tex2DNode38 = UnpackNormal( tex2D( _SkinNormal, uv_SkinNormal ) );
			float2 temp_cast_0 = (_SkinPore_Tilling).xx;
			float2 uv2_TexCoord94 = i.uv2_texcoord2 * temp_cast_0;
			float3 temp_output_92_0 = BlendNormals( tex2DNode38 , UnpackScaleNormal( tex2D( _SkinPore, uv2_TexCoord94 ), _SkinPore_Power ) );
			float2 uv_AMS = i.uv_texcoord * _AMS_ST.xy + _AMS_ST.zw;
			float4 tex2DNode220 = tex2D( _AMS, uv_AMS );
			float AMSAlpha221 = tex2DNode220.r;
			float3 lerpResult234 = lerp( temp_output_92_0 , tex2DNode38 , AMSAlpha221);
			#ifdef _ISCYBORG_ON
				float3 staticSwitch233 = lerpResult234;
			#else
				float3 staticSwitch233 = temp_output_92_0;
			#endif
			float3 Normal193 = staticSwitch233;
			o.Normal = Normal193;
			float4 saferPower136 = max( _BaseColorMul , 0.0001 );
			float4 temp_cast_1 = (_ColorPow).xxxx;
			float2 uv_SkinColor = i.uv_texcoord * _SkinColor_ST.xy + _SkinColor_ST.zw;
			float4 tex2DNode4 = tex2D( _SkinColor, uv_SkinColor );
			float4 temp_output_137_0 = ( pow( saferPower136 , temp_cast_1 ) * tex2DNode4 );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode11 = tex2D( _Mask, uv_Mask );
			float4 lerpResult124 = lerp( float4( 0,0,0,0 ) , _LipNailColor , tex2DNode11.r);
			float4 lerpResult125 = lerp( float4( 0,0,0,0 ) , _FreaklesColor , tex2DNode11.g);
			float4 lerpResult126 = lerp( float4( 0,0,0,0 ) , _HairCapColor , tex2DNode11.b);
			float4 temp_output_57_0 = ( lerpResult124 + lerpResult125 + lerpResult126 );
			float temp_output_123_0 = ( ( _LipNailColorPower * tex2DNode11.r ) + ( _FreaklesColorPower * tex2DNode11.g ) + ( _HairCapColorPower * tex2DNode11.b ) );
			float clampResult165 = clamp( temp_output_123_0 , 0.0 , 1.0 );
			float4 lerpResult140 = lerp( temp_output_137_0 , temp_output_57_0 , clampResult165);
			float2 uv_ExtraMask = i.uv_texcoord * _ExtraMask_ST.xy + _ExtraMask_ST.zw;
			float4 tex2DNode202 = tex2D( _ExtraMask, uv_ExtraMask );
			float4 lerpResult213 = lerp( float4( 0,0,0,0 ) , _EyeShadowOutColor , tex2DNode202.r);
			float4 lerpResult211 = lerp( float4( 0,0,0,0 ) , _EyeShadowInColor , tex2DNode202.g);
			float4 lerpResult214 = lerp( float4( 0,0,0,0 ) , _BlushColor , tex2DNode202.b);
			float clampResult216 = clamp( ( ( _EyeShadowOutPower * tex2DNode202.r ) + ( _EyeShadowInPower * tex2DNode202.g ) + ( _BlushPower * tex2DNode202.b ) + temp_output_123_0 ) , 0.0 , 1.0 );
			float4 lerpResult217 = lerp( temp_output_137_0 , ( lerpResult213 + lerpResult211 + lerpResult214 + temp_output_57_0 ) , clampResult216);
			#ifdef _ISEXTRAMASKEUP_ON
				float4 staticSwitch218 = lerpResult217;
			#else
				float4 staticSwitch218 = lerpResult140;
			#endif
			float2 uv_TattoMask = i.uv_texcoord * _TattoMask_ST.xy + _TattoMask_ST.zw;
			float4 lerpResult187 = lerp( staticSwitch218 , _TattooColor , ( tex2D( _TattoMask, uv_TattoMask ).r * _TattooBoots ));
			float4 clampResult188 = clamp( lerpResult187 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 SkinColor241 = ( tex2DNode4 * ( 1.0 - _MetallicMultiply ) );
			float4 lerpResult237 = lerp( clampResult188 , SkinColor241 , AMSAlpha221);
			#ifdef _ISCYBORG_ON
				float4 staticSwitch236 = lerpResult237;
			#else
				float4 staticSwitch236 = clampResult188;
			#endif
			float2 uv_Dirt = i.uv_texcoord * _Dirt_ST.xy + _Dirt_ST.zw;
			float4 tex2DNode63 = tex2D( _Dirt, uv_Dirt );
			float4 lerpResult70 = lerp( staticSwitch236 , ( _DirtBoots * _DirtColor ) , ( _DirtBoots * tex2DNode63.r ));
			float4 Albedo191 = lerpResult70;
			o.Albedo = Albedo191.rgb;
			float2 uv_EmissiveMask = i.uv_texcoord * _EmissiveMask_ST.xy + _EmissiveMask_ST.zw;
			float4 tex2DNode163 = tex2D( _EmissiveMask, uv_EmissiveMask );
			float2 temp_cast_3 = (_EmissivePannerTilling).xx;
			float2 uv_TexCoord142 = i.uv_texcoord * temp_cast_3;
			float2 panner145 = ( _Time.y * _PannerProperty + uv_TexCoord142);
			float4 tex2DNode153 = tex2D( _EmissivePannerMap, panner145 );
			float4 Emmision195 = ( _Emissivepowerboost * ( ( _EmissiveColor1 * _EmissivePower1 * tex2DNode163.r * tex2DNode153 ) + ( _EmissiveColor2 * _EmissivePower2 * tex2DNode163.g * tex2DNode153 ) + ( _EmissiveColor3 * _EmissivePower3 * tex2DNode163.b * tex2DNode153 ) ) );
			o.Emission = Emmision195.rgb;
			float lerpResult224 = lerp( 0.0 , tex2DNode220.g , tex2DNode220.r);
			#ifdef _ISCYBORG_ON
				float staticSwitch222 = lerpResult224;
			#else
				float staticSwitch222 = 0.0;
			#endif
			float Metallic225 = staticSwitch222;
			o.Metallic = Metallic225;
			float2 uv_ORM = i.uv_texcoord * _ORM_ST.xy + _ORM_ST.zw;
			float4 tex2DNode173 = tex2D( _ORM, uv_ORM );
			float2 uv_RoughnessMod_R = i.uv_texcoord * _RoughnessMod_R_ST.xy + _RoughnessMod_R_ST.zw;
			float lerpResult174 = lerp( ( tex2DNode173.g * ( _SkinRounghnessBoots * tex2D( _RoughnessMod_R, uv_RoughnessMod_R ).a ) ) , tex2DNode173.b , 0.0);
			float lerpResult71 = lerp( lerpResult174 , ( _DirtRoughness_Power * _DirtBoots ) , ( _DirtBoots * tex2DNode63.r ));
			float lerpResult170 = lerp( lerpResult71 , _HairCapRoughness , tex2DNode11.b);
			float lerpResult141 = lerp( lerpResult170 , _LipsNailRoughnessBoost , tex2DNode11.r);
			float clampResult171 = clamp( lerpResult141 , 0.0 , 1.0 );
			float TrueRoughness245 = ( 1.0 - tex2DNode173.g );
			float lerpResult247 = lerp( clampResult171 , TrueRoughness245 , AMSAlpha221);
			#ifdef _ISCYBORG_ON
				float staticSwitch244 = lerpResult247;
			#else
				float staticSwitch244 = clampResult171;
			#endif
			float Smoothness189 = staticSwitch244;
			o.Smoothness = Smoothness189;
			float AO197 = tex2DNode173.r;
			o.Occlusion = AO197;
			float4 temp_output_176_0 = ( _SSSColor * ( 1.0 - tex2DNode173.b ) );
			float4 temp_cast_5 = (0.0).xxxx;
			float4 lerpResult229 = lerp( temp_output_176_0 , temp_cast_5 , AMSAlpha221);
			#ifdef _ISCYBORG_ON
				float4 staticSwitch227 = lerpResult229;
			#else
				float4 staticSwitch227 = temp_output_176_0;
			#endif
			float4 Transmission199 = staticSwitch227;
			o.Transmission = Transmission199.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
2560;0;2560;1059;-307.5859;2274.14;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;117;-1972.546,-741.9335;Inherit;False;Property;_FreaklesColorPower;Freakles Color Power;26;0;Create;True;0;0;0;False;0;False;0;0;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-1972.546,-869.9335;Inherit;False;Property;_LipNailColorPower;Lip/Nail Color Power;23;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-2007.614,-1175.961;Inherit;True;Property;_Mask;Mask;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;115;-1972.546,-581.9335;Inherit;False;Property;_HairCapColorPower;HairCap Color Power;28;0;Create;True;0;0;0;False;0;False;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-702.4037,-1321.148;Float;False;Property;_HairCapColor;HairCap Color;27;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-1636.403,-871.0615;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;202;-1970.041,-1986.437;Inherit;True;Property;_ExtraMask;Extra Mask;31;0;Create;True;0;0;0;False;0;False;-1;None;937599cb8c3c1b748842feed92c8ad35;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;203;-1981.461,-1533.622;Inherit;False;Property;_EyeShadowInPower;Eye Shadow In Power;35;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-964.546,-1317.933;Float;False;Property;_FreaklesColor;Freakles Color;25;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-1652.546,-597.9335;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;201;-1958.428,-1398.741;Inherit;False;Property;_BlushPower;Blush Power;37;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-1364.546,-1301.933;Float;False;Property;_LipNailColor;Lip/Nail Color;22;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5566037,0.107645,0.107645,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;204;-1981.64,-1666.064;Inherit;False;Property;_EyeShadowOutPower;Eye Shadow Out Power;33;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-1636.546,-725.9335;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;125;-852.546,-1093.933;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-1644.998,-1383.795;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;126;-644.546,-1093.933;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-1413.534,406.7604;Inherit;True;Property;_RoughnessMod_R;RoughnessMod_R;16;0;Create;True;0;0;0;False;0;False;-1;c0b7b18616e3d4c4d9f007d6681e9331;c0b7b18616e3d4c4d9f007d6681e9331;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-1646.21,-1668.414;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-1365.233,-896.09;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;205;-693.5884,-2096.865;Float;False;Property;_BlushColor;Blush Color;36;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-1412.546,-677.9335;Float;False;Property;_BaseColorMul;BaseColorMul;20;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.372549,0.4821379,0.6509804,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;124;-1076.546,-1093.933;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-1645.749,-1537.139;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;207;-1371.712,-2094.446;Float;False;Property;_EyeShadowOutColor;Eye Shadow Out  Color;32;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5566037,0.107645,0.107645,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-1409.906,299.3792;Float;False;Property;_SkinRounghnessBoots;Skin Rounghness Boots;15;0;Create;True;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1410.582,-202.3145;Float;False;Property;_ColorPow;Color Pow;21;0;Create;True;0;0;0;False;0;False;0;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;209;-970.5338,-2104.97;Float;False;Property;_EyeShadowInColor;Eye Shadow In  Color;34;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;136;-1054.79,-512.5454;Inherit;False;True;2;0;COLOR;2,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-1431.925,-404.8359;Inherit;True;Property;_SkinColor;Skin Color;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;166;-2243.772,-1352.454;Inherit;False;Constant;_min;min;40;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;213;-1086.587,-1882.073;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2239.772,-1279.454;Inherit;False;Constant;_max;max;40;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;214;-650.5861,-1885.073;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-676.546,-869.9335;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1046.182,390.2917;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;173;-1652.475,58.52325;Inherit;True;Property;_ORM;ORM;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;211;-856.5855,-1882.073;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-1363.705,-1643.223;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;239;-1408.329,-122.5664;Inherit;False;Property;_MetallicMultiply;Metallic Multiply;14;0;Create;True;0;0;0;False;0;False;0.35;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;165;-725.5779,-640.1586;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;216;-918.5694,-1648.778;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;215;-354.4595,-1877.649;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;63;178.0533,-2521.144;Inherit;True;Property;_Dirt;Dirt;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;97;221.9754,-2323.193;Float;False;Property;_DirtRoughness_Power;DirtRoughness_Power;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;198.3474,-2631.708;Float;False;Property;_DirtBoots;Dirt Boots;5;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-868.0598,-418.015;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-899.0429,278.6664;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;184;569.3889,-2859.885;Inherit;True;Property;_TattoMask;Tatto Mask;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;589.683,-2970.448;Float;False;Property;_TattooBoots;Tattoo Boots;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;217;12.18595,-1873.014;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;243;-1104.172,-89.22876;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;140;-500.5461,-581.9335;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;174;-720.5175,100.4184;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;590.8161,-2382.506;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;590.8161,-2253.322;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-4191.182,-571.95;Inherit;False;Property;_EmissivePannerTilling;Emissive Panner Tilling;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;944.5994,-2942.707;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;144;-3905.697,-282.8923;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;220;-3488.457,484.374;Inherit;True;Property;_AMS;AMS;13;0;Create;False;0;0;0;False;0;False;-1;6adfd6e42e50e114c9b30f7df5c8e037;6adfd6e42e50e114c9b30f7df5c8e037;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;185;648.1018,-3155.084;Inherit;False;Property;_TattooColor;Tattoo Color;1;0;Create;True;0;0;0;False;0;False;0.1422659,0.1889629,0.2169811,0;0.1509434,0.0874304,0.05339979,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-949.3293,-184.5664;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2216.442,839.7252;Inherit;False;Property;_SkinPore_Tilling;SkinPore_Tilling;18;0;Create;True;0;0;0;False;0;False;35;35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;218;353.7222,-1861.882;Inherit;False;Property;_IsExtraMaskeUp;Is Extra MaskeUp;30;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;142;-3921.697,-570.8915;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;143;-3889.697,-442.8915;Float;False;Property;_PannerProperty;Panner Property;48;0;Create;True;0;0;0;False;0;False;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;71;1016.849,-2355.361;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;1276.014,-1769.36;Inherit;False;Property;_HairCapRoughness;HairCap Roughness;29;0;Create;True;0;0;0;False;0;False;0.6;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2279.465,930.3577;Inherit;False;Property;_SkinPore_Power;SkinPore_Power;19;0;Create;True;0;0;0;False;0;False;0.25;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-2010.437,837.6755;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;187;1450.095,-3179.282;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;145;-3601.696,-410.8915;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;246;-1263.745,107.5792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;221;-3170.112,458.9703;Inherit;False;AMSAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;1325.632,-1903.585;Inherit;False;Property;_LipsNailRoughnessBoost;Lips/Nail Roughness Boost;24;0;Create;True;0;0;0;False;0;False;0.75;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;170;1608.302,-1722.595;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;241;-745.3293,-187.5664;Inherit;False;SkinColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;141;1809.588,-1763.93;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;148;-2852.855,-212.5536;Float;False;Property;_EmissiveColor2;Emissive Color 2;42;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-2853.01,42.5166;Float;False;Property;_EmissiveColor3;Emissive Color 3;44;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;242;1738.292,-2842.864;Inherit;False;241;SkinColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;238;1737.86,-2765.444;Inherit;False;221;AMSAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-2922.544,-300.5449;Float;False;Property;_EmissivePower1;Emissive Power 1;41;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-2924.894,216.7375;Float;False;Property;_EmissivePower3;Emissive Power 3;45;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-2919.855,-39.55336;Float;False;Property;_EmissivePower2;Emissive Power 2;43;0;Create;True;0;0;0;False;0;False;0;0;0;2000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;-1713.906,637.6068;Inherit;True;Property;_SkinNormal;Skin Normal;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;177;-522.1078,131.2842;Inherit;False;Property;_SSSColor;SSS Color;11;0;Create;True;0;0;0;False;0;False;1,0.7457517,0.6839622,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;113;-488.9619,407.2928;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;163;-3304.605,-876.3577;Inherit;True;Property;_EmissiveMask;Emissive Mask;38;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;93;-1712.19,842.8002;Inherit;True;Property;_SkinPore;Skin Pore;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;154;-2856.544,-472.545;Float;False;Property;_EmissiveColor1;Emissive Color 1;40;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;153;-3329.696,-410.8915;Inherit;True;Property;_EmissivePannerMap;Emissive Panner Map;46;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;245;-1097.745,49.57916;Inherit;False;TrueRoughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;188;1639.54,-3180.625;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-2255.959,-220.9686;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;1902.586,-1542.14;Inherit;False;221;AMSAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;-389.7534,567.6797;Inherit;False;Constant;_SSSZero;SSSZero;49;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;237;2069.86,-2854.444;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-263.8493,331.0256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;235;-1295.464,824.4758;Inherit;False;221;AMSAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;171;1996.329,-1921.962;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-2269.648,-493.9601;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;92;-1368.849,635.77;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;1875.959,-1617.38;Inherit;False;245;TrueRoughness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;219;249.4496,-2805.346;Inherit;False;Property;_DirtColor;Dirt Color;4;0;Create;True;0;0;0;False;0;False;0.1509434,0.0874304,0.05339979,0;0.1509434,0.0874304,0.05339979,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;231;-391.9456,653.419;Inherit;False;221;AMSAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-3369.784,728.4648;Inherit;False;Constant;_MetallicZero;MetallicZero;48;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-2251.683,51.29242;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;236;2260.86,-2894.444;Inherit;False;Property;_isCyborg;is Cyborg;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;224;-3107.784,829.4648;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-2073.772,96.72601;Inherit;False;Property;_Emissivepowerboost;Emissive power boost;39;0;Create;True;0;0;0;False;0;False;100;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;590.816,-2633;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-2039.662,-169.42;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;229;-126.9884,518.0754;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;587.6652,-2506.965;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;247;2130.358,-1640.58;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;234;-1095.34,740.5292;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;244;2269.744,-1915.15;Inherit;False;Property;_isCyborg;is Cyborg;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;222;-2919.27,732.1074;Inherit;False;Property;_isCyborg;is Cyborg;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;227;62.2229,447.2637;Inherit;False;Property;_isCyborg;is Cyborg;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;233;-934.3755,637.9788;Inherit;False;Property;_isCyborg;is Cyborg;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;70;2296.632,-2738.562;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-1805.381,-40.5952;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;-1341.544,-23.09039;Inherit;False;AO;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-696.8765,640.1403;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;189;2532.788,-1927.707;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-1659.344,-45.2764;Inherit;False;Emmision;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;296.6809,321.9385;Inherit;False;Transmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;2575.039,-2736.114;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;225;-2689.106,733.6825;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;1971.237,-723.4036;Inherit;False;195;Emmision;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;1967.774,-882.3823;Inherit;False;191;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;190;1965.301,-532.9734;Inherit;False;189;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;1962.912,-457.4102;Inherit;False;199;Transmission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;226;1971.061,-628.7299;Inherit;False;225;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;1966.401,-379.5916;Inherit;False;197;AO;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;1967.805,-803.0275;Inherit;False;193;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;172;2222.238,-748.6663;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;DawnShader_V2/Skin;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;119;0;116;0
WireConnection;119;1;11;1
WireConnection;121;0;115;0
WireConnection;121;1;11;3
WireConnection;120;0;117;0
WireConnection;120;1;11;2
WireConnection;125;1;20;0
WireConnection;125;2;11;2
WireConnection;208;0;201;0
WireConnection;208;1;202;3
WireConnection;126;1;21;0
WireConnection;126;2;11;3
WireConnection;210;0;204;0
WireConnection;210;1;202;1
WireConnection;123;0;119;0
WireConnection;123;1;120;0
WireConnection;123;2;121;0
WireConnection;124;1;12;0
WireConnection;124;2;11;1
WireConnection;206;0;203;0
WireConnection;206;1;202;2
WireConnection;136;0;7;0
WireConnection;136;1;9;0
WireConnection;213;1;207;0
WireConnection;213;2;202;1
WireConnection;214;1;205;0
WireConnection;214;2;202;3
WireConnection;57;0;124;0
WireConnection;57;1;125;0
WireConnection;57;2;126;0
WireConnection;44;0;26;0
WireConnection;44;1;3;4
WireConnection;211;1;209;0
WireConnection;211;2;202;2
WireConnection;212;0;210;0
WireConnection;212;1;206;0
WireConnection;212;2;208;0
WireConnection;212;3;123;0
WireConnection;165;0;123;0
WireConnection;165;1;166;0
WireConnection;165;2;167;0
WireConnection;216;0;212;0
WireConnection;216;1;166;0
WireConnection;216;2;167;0
WireConnection;215;0;213;0
WireConnection;215;1;211;0
WireConnection;215;2;214;0
WireConnection;215;3;57;0
WireConnection;137;0;136;0
WireConnection;137;1;4;0
WireConnection;43;0;173;2
WireConnection;43;1;44;0
WireConnection;217;0;137;0
WireConnection;217;1;215;0
WireConnection;217;2;216;0
WireConnection;243;0;239;0
WireConnection;140;0;137;0
WireConnection;140;1;57;0
WireConnection;140;2;165;0
WireConnection;174;0;43;0
WireConnection;174;1;173;3
WireConnection;69;0;97;0
WireConnection;69;1;65;0
WireConnection;66;0;65;0
WireConnection;66;1;63;1
WireConnection;186;0;184;1
WireConnection;186;1;183;0
WireConnection;240;0;4;0
WireConnection;240;1;243;0
WireConnection;218;1;140;0
WireConnection;218;0;217;0
WireConnection;142;0;162;0
WireConnection;71;0;174;0
WireConnection;71;1;69;0
WireConnection;71;2;66;0
WireConnection;94;0;95;0
WireConnection;187;0;218;0
WireConnection;187;1;185;0
WireConnection;187;2;186;0
WireConnection;145;0;142;0
WireConnection;145;2;143;0
WireConnection;145;1;144;2
WireConnection;246;0;173;2
WireConnection;221;0;220;1
WireConnection;170;0;71;0
WireConnection;170;1;169;0
WireConnection;170;2;11;3
WireConnection;241;0;240;0
WireConnection;141;0;170;0
WireConnection;141;1;128;0
WireConnection;141;2;11;1
WireConnection;113;0;173;3
WireConnection;93;1;94;0
WireConnection;93;5;96;0
WireConnection;153;1;145;0
WireConnection;245;0;246;0
WireConnection;188;0;187;0
WireConnection;157;0;148;0
WireConnection;157;1;149;0
WireConnection;157;2;163;2
WireConnection;157;3;153;0
WireConnection;237;0;188;0
WireConnection;237;1;242;0
WireConnection;237;2;238;0
WireConnection;176;0;177;0
WireConnection;176;1;113;0
WireConnection;171;0;141;0
WireConnection;171;1;166;0
WireConnection;171;2;167;0
WireConnection;155;0;154;0
WireConnection;155;1;147;0
WireConnection;155;2;163;1
WireConnection;155;3;153;0
WireConnection;92;0;38;0
WireConnection;92;1;93;0
WireConnection;158;0;152;0
WireConnection;158;1;150;0
WireConnection;158;2;163;3
WireConnection;158;3;153;0
WireConnection;236;1;188;0
WireConnection;236;0;237;0
WireConnection;224;0;223;0
WireConnection;224;1;220;2
WireConnection;224;2;220;1
WireConnection;68;0;65;0
WireConnection;68;1;219;0
WireConnection;159;0;155;0
WireConnection;159;1;157;0
WireConnection;159;2;158;0
WireConnection;229;0;176;0
WireConnection;229;1;232;0
WireConnection;229;2;231;0
WireConnection;67;0;65;0
WireConnection;67;1;63;1
WireConnection;247;0;171;0
WireConnection;247;1;248;0
WireConnection;247;2;249;0
WireConnection;234;0;92;0
WireConnection;234;1;38;0
WireConnection;234;2;235;0
WireConnection;244;1;171;0
WireConnection;244;0;247;0
WireConnection;222;1;223;0
WireConnection;222;0;224;0
WireConnection;227;1;176;0
WireConnection;227;0;229;0
WireConnection;233;1;92;0
WireConnection;233;0;234;0
WireConnection;70;0;236;0
WireConnection;70;1;68;0
WireConnection;70;2;67;0
WireConnection;161;0;160;0
WireConnection;161;1;159;0
WireConnection;197;0;173;1
WireConnection;193;0;233;0
WireConnection;189;0;244;0
WireConnection;195;0;161;0
WireConnection;199;0;227;0
WireConnection;191;0;70;0
WireConnection;225;0;222;0
WireConnection;172;0;192;0
WireConnection;172;1;194;0
WireConnection;172;2;196;0
WireConnection;172;3;226;0
WireConnection;172;4;190;0
WireConnection;172;5;198;0
WireConnection;172;6;200;0
ASEEND*/
//CHKSM=7A63B94BA7919A4722AA2A2ACC63A5D9F686FB3A