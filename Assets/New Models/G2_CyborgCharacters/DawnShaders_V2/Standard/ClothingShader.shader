// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DawnShader_V2/Clothing"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_Dirt("Dirt", 2D) = "white" {}
		_DirtBoots("Dirt Boots", Range( 0 , 2)) = 0
		_DirtRoughness("Dirt Roughness", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		_DirtColor("Dirt Color", Color) = (0.08490568,0.04946156,0.01401745,1)
		_Mask("Mask", 2D) = "white" {}
		_ORC("ORC", 2D) = "white" {}
		_MetallicBoots01("Metallic Boots 01", Range( 0 , 4)) = 1
		_MetallicBoots02("Metallic Boots 02", Range( 0 , 4)) = 1
		_MetallicBoots03("Metallic Boots 03", Range( 0 , 4)) = 0
		_RoughnessBoost01("Roughness Boost 01", Range( 0 , 4)) = 1
		_RoughnessBoost02("Roughness Boost 02", Range( 0 , 4)) = 1
		_RoughnessBoost03("Roughness Boost 03", Range( 0 , 4)) = 1
		_BC("Base Color", 2D) = "white" {}
		_BlendColor01("Blend Color 01", Color) = (1,1,1,0)
		_BlendColorPower01("Blend Color Power 01", Range( 0 , 1)) = 0
		_BlendColor02("Blend Color 02", Color) = (1,1,1,0)
		_BlendColorPower02("Blend Color Power 02", Range( 0 , 1)) = 0
		_BlendColor03("Blend Color 03", Color) = (1,1,1,0)
		_BlendColorPower03("Blend Color Power 03", Range( 0 , 1)) = 0
		[Toggle(_BLENDTEXTUREMODE_ON)] _BlendTextureMode("Blend Texture Mode", Float) = 0
		[Toggle(_BLENDTEXTURE01_ON)] _BlendTexture01("Blend Texture 01", Float) = 0
		[Toggle(_BLENDTEXTURE02_ON)] _BlendTexture02("Blend Texture 02", Float) = 0
		[Toggle(_BLENDTEXTURE03_ON)] _BlendTexture03("Blend Texture 03", Float) = 0
		_Texture01("Texture 01", 2D) = "white" {}
		_Texture02("Texture 02", 2D) = "white" {}
		_Texture03("Texture 03", 2D) = "white" {}
		_EmissiveMask1("Emissive Mask", 2D) = "white" {}
		_EmissiveColor2("Emissive Color 01", Color) = (0,0,0,0)
		_EmissiveBoots2("Emissive Boots 1", Float) = 0
		_EmissiveColor3("Emissive Color 02", Color) = (0,0,0,0)
		_EmissiveBoots3("Emissive Boots 2", Float) = 0
		_EmissiveColor4("Emissive Color 03", Color) = (0,0,0,0)
		_EmissiveBoots4("Emissive Boots 3", Float) = 0
		_EmissivePannerMap1("Emissive Panner Map", 2D) = "white" {}
		_EmissivePannerTilling1("Emissive Panner Tilling", Float) = 1
		_PannerProperty1("Panner Property", Vector) = (0,0,0,0)
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
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _BLENDTEXTUREMODE_ON
		#pragma shader_feature _BLENDTEXTURE01_ON
		#pragma shader_feature _BLENDTEXTURE02_ON
		#pragma shader_feature _BLENDTEXTURE03_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _BC;
		uniform float4 _BC_ST;
		uniform float4 _BlendColor01;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float4 _BlendColor02;
		uniform float4 _BlendColor03;
		uniform float _BlendColorPower01;
		uniform float _BlendColorPower02;
		uniform float _BlendColorPower03;
		uniform sampler2D _Texture01;
		uniform float4 _Texture01_ST;
		uniform sampler2D _Texture02;
		uniform float4 _Texture02_ST;
		uniform sampler2D _Texture03;
		uniform float4 _Texture03_ST;
		uniform float4 _DirtColor;
		uniform float _DirtBoots;
		uniform sampler2D _Dirt;
		uniform float4 _Dirt_ST;
		uniform sampler2D _EmissiveMask1;
		uniform float4 _EmissiveMask1_ST;
		uniform sampler2D _EmissivePannerMap1;
		uniform float2 _PannerProperty1;
		uniform float _EmissivePannerTilling1;
		uniform float _EmissiveBoots2;
		uniform float4 _EmissiveColor2;
		uniform float _EmissiveBoots3;
		uniform float4 _EmissiveColor3;
		uniform float _EmissiveBoots4;
		uniform float4 _EmissiveColor4;
		uniform sampler2D _ORC;
		uniform float4 _ORC_ST;
		uniform float _MetallicBoots01;
		uniform float _MetallicBoots02;
		uniform float _MetallicBoots03;
		uniform float _RoughnessBoost01;
		uniform float _RoughnessBoost02;
		uniform float _RoughnessBoost03;
		uniform float _DirtRoughness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_BC = i.uv_texcoord * _BC_ST.xy + _BC_ST.zw;
			float4 tex2DNode1 = tex2D( _BC, uv_BC );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode2 = tex2D( _Mask, uv_Mask );
			float4 lerpResult28 = lerp( float4( 0,0,0,0 ) , _BlendColor01 , tex2DNode2.r);
			float4 lerpResult29 = lerp( float4( 0,0,0,0 ) , _BlendColor02 , tex2DNode2.g);
			float4 lerpResult30 = lerp( float4( 0,0,0,0 ) , _BlendColor03 , tex2DNode2.b);
			float4 lerpResult132 = lerp( tex2DNode1 , ( lerpResult28 + lerpResult29 + lerpResult30 ) , ( ( _BlendColorPower01 * tex2DNode2.r ) + ( _BlendColorPower02 * tex2DNode2.g ) + ( _BlendColorPower03 * tex2DNode2.b ) ));
			float2 uv_Texture01 = i.uv_texcoord * _Texture01_ST.xy + _Texture01_ST.zw;
			float4 lerpResult55 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture01, uv_Texture01 ) , tex2DNode2.r);
			#ifdef _BLENDTEXTURE01_ON
				float4 staticSwitch60 = lerpResult55;
			#else
				float4 staticSwitch60 = tex2DNode1;
			#endif
			float2 uv_Texture02 = i.uv_texcoord * _Texture02_ST.xy + _Texture02_ST.zw;
			float4 lerpResult56 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture02, uv_Texture02 ) , tex2DNode2.g);
			#ifdef _BLENDTEXTURE02_ON
				float4 staticSwitch61 = lerpResult56;
			#else
				float4 staticSwitch61 = tex2DNode1;
			#endif
			float2 uv_Texture03 = i.uv_texcoord * _Texture03_ST.xy + _Texture03_ST.zw;
			float4 lerpResult57 = lerp( float4( 0,0,0,0 ) , tex2D( _Texture03, uv_Texture03 ) , tex2DNode2.b);
			#ifdef _BLENDTEXTURE03_ON
				float4 staticSwitch62 = lerpResult57;
			#else
				float4 staticSwitch62 = tex2DNode1;
			#endif
			float temp_output_27_0 = ( tex2DNode2.r + tex2DNode2.g + tex2DNode2.b + tex2DNode2.a );
			float4 lerpResult71 = lerp( tex2DNode1 , ( ( staticSwitch60 * tex2DNode2.r ) + ( staticSwitch61 * tex2DNode2.g ) + ( staticSwitch62 * tex2DNode2.b ) ) , temp_output_27_0);
			#ifdef _BLENDTEXTUREMODE_ON
				float4 staticSwitch72 = lerpResult71;
			#else
				float4 staticSwitch72 = lerpResult132;
			#endif
			float2 uv_Dirt = i.uv_texcoord * _Dirt_ST.xy + _Dirt_ST.zw;
			float4 tex2DNode80 = tex2D( _Dirt, uv_Dirt );
			float4 lerpResult87 = lerp( staticSwitch72 , ( _DirtColor * _DirtBoots ) , ( _DirtBoots * tex2DNode80.a ));
			o.Albedo = lerpResult87.rgb;
			float2 uv_EmissiveMask1 = i.uv_texcoord * _EmissiveMask1_ST.xy + _EmissiveMask1_ST.zw;
			float4 tex2DNode155 = tex2D( _EmissiveMask1, uv_EmissiveMask1 );
			float2 temp_cast_1 = (_EmissivePannerTilling1).xx;
			float2 uv_TexCoord150 = i.uv_texcoord * temp_cast_1;
			float2 panner152 = ( _Time.y * _PannerProperty1 + uv_TexCoord150);
			float4 tex2DNode153 = tex2D( _EmissivePannerMap1, panner152 );
			o.Emission = ( ( tex2DNode155.r * tex2DNode153 * _EmissiveBoots2 * _EmissiveColor2 ) + ( tex2DNode155.g * tex2DNode153 * _EmissiveBoots3 * _EmissiveColor3 ) + ( tex2DNode155.b * tex2DNode153 * _EmissiveBoots4 * _EmissiveColor4 ) ).rgb;
			float2 uv_ORC = i.uv_texcoord * _ORC_ST.xy + _ORC_ST.zw;
			float4 tex2DNode168 = tex2D( _ORC, uv_ORC );
			float lerpResult128 = lerp( tex2DNode168.b , ( ( ( _MetallicBoots01 * tex2DNode2.r ) + ( _MetallicBoots02 * tex2DNode2.g ) + ( _MetallicBoots03 * tex2DNode2.b ) ) * tex2DNode168.b ) , temp_output_27_0);
			o.Metallic = lerpResult128;
			float temp_output_147_0 = ( 1.0 - tex2DNode168.g );
			float lerpResult119 = lerp( temp_output_147_0 , ( ( ( _RoughnessBoost01 * tex2DNode2.r ) + ( _RoughnessBoost02 * tex2DNode2.g ) + ( _RoughnessBoost03 * tex2DNode2.b ) ) * temp_output_147_0 ) , temp_output_27_0);
			float lerpResult88 = lerp( lerpResult119 , ( _DirtRoughness * _DirtBoots ) , ( _DirtBoots * tex2DNode80.a ));
			o.Smoothness = lerpResult88;
			o.Occlusion = tex2DNode168.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
2560;0;2560;1059;6628.572;4065.041;3.949747;True;False
Node;AmplifyShaderEditor.SamplerNode;50;-2882.945,273.4547;Inherit;True;Property;_Texture03;Texture 03;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-2881.104,4.529496;Inherit;True;Property;_Texture02;Texture 02;25;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-3535.044,-1144.979;Inherit;True;Property;_Mask;Mask;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;39;-2885.036,-281.1397;Inherit;True;Property;_Texture01;Texture 01;24;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;56;-2486.201,-16.73898;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-3525.175,-949.9409;Inherit;True;Property;_BC;Base Color;13;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;57;-2468.646,256.5266;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;-2491.789,-300.0495;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;60;-2213.897,-327.3228;Float;False;Property;_BlendTexture01;Blend Texture 01;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-333.8505,-1052.04;Float;False;Property;_RoughnessBoost01;Roughness Boost 01;10;0;Create;True;0;0;0;False;0;False;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-3896.215,-1813.283;Inherit;False;Property;_BlendColorPower02;Blend Color Power 02;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-3892.48,-1661.64;Inherit;False;Property;_BlendColorPower03;Blend Color Power 03;19;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-331.6754,-916.2148;Float;False;Property;_RoughnessBoost02;Roughness Boost 02;11;0;Create;True;0;0;0;False;0;False;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-3897.688,-1945.725;Inherit;False;Property;_BlendColorPower01;Blend Color Power 01;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-2692.885,-1341.029;Float;False;Property;_BlendColor03;Blend Color 03;18;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;148;-3547.447,-2794.011;Inherit;False;Property;_EmissivePannerTilling1;Emissive Panner Tilling;35;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;62;-2210.348,-111.9511;Float;False;Property;_BlendTexture03;Blend Texture 03;23;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;61;-2210.348,-219.3009;Float;False;Property;_BlendTexture02;Blend Texture 02;22;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-337.6754,-792.2153;Float;False;Property;_RoughnessBoost03;Roughness Boost 03;12;0;Create;True;0;0;0;False;0;False;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-2912.896,-1335.285;Float;False;Property;_BlendColor02;Blend Color 02;16;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-3124.891,-1335.198;Float;False;Property;_BlendColor01;Blend Color 01;14;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-10.13867,-1078.428;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-43.93866,-917.8286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-3583.226,-1664.936;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;-2976.758,-2038.651;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1853.199,-282.3062;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-36.93866,-790.8286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;28;-2976.32,-2171.03;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-3561.797,-1803.495;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;511.2816,-778.0138;Float;False;Property;_MetallicBoots03;Metallic Boots 03;9;0;Create;True;0;0;0;False;0;False;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;168;-975.61,-1496.543;Inherit;True;Property;_ORC;ORC;6;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1853.199,-516.6637;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;30;-2972.165,-1909.679;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;102;516.2817,-902.0133;Float;False;Property;_MetallicBoots02;Metallic Boots 02;8;0;Create;True;0;0;0;False;0;False;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;150;-3247.01,-2796.18;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;149;-3231.01,-2508.181;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;151;-3215.01,-2668.18;Float;False;Property;_PannerProperty1;Panner Property;36;0;Create;True;0;0;0;False;0;False;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;101;514.1066,-1037.839;Float;False;Property;_MetallicBoots01;Metallic Boots 01;7;0;Create;True;0;0;0;False;0;False;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1854.932,-396.8809;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-3561.463,-1947.405;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;152;-2927.01,-2636.18;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-3316.564,-1935.351;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-161.379,-477.6454;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;811.5206,-904.7785;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;146;-2333.468,-1963.133;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;118;261.8468,-872.3459;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;803.5206,-1036.779;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;818.5206,-777.7785;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-2508.949,-916.8371;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-1467.807,-348.2731;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-2796.186,-3612.935;Float;False;Property;_EmissiveBoots4;Emissive Boots 3;33;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;153;-2655.01,-2636.18;Inherit;True;Property;_EmissivePannerMap1;Emissive Panner Map;34;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;169;-408.988,-1312.631;Float;False;Property;_DirtRoughness;Dirt Roughness;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;156;-3087.453,-3342.711;Float;False;Property;_EmissiveColor3;Emissive Color 02;30;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.6797385,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;144.0613,-552.8286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-484.8184,-1625.895;Float;False;Property;_DirtBoots;Dirt Boots;1;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;127;1117.306,-859.2958;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;157;-3094.817,-3511.71;Float;False;Property;_EmissiveColor2;Emissive Color 01;28;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4009434,0.8080564,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;80;-505.1128,-1515.332;Inherit;True;Property;_Dirt;Dirt;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;162;-3081.238,-3178.25;Float;False;Property;_EmissiveColor4;Emissive Color 03;32;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4009434,0.8080564,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;155;-2801.94,-2895.825;Inherit;True;Property;_EmissiveMask1;Emissive Mask;27;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;160;-2793.772,-3776.639;Float;False;Property;_EmissiveBoots2;Emissive Boots 1;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-2795.918,-3691.973;Float;False;Property;_EmissiveBoots3;Emissive Boots 2;31;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;132;-1686.949,-1717.876;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;170;-438.1747,-1821.044;Inherit;False;Property;_DirtColor;Dirt Color;4;0;Create;False;0;0;0;False;0;False;0.08490568,0.04946156,0.01401745,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;71;-1243.507,-392.5125;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-2175.48,-3525.295;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-95.50151,-1501.153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-2156.459,-3177.771;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-92.35083,-1627.187;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;72;-765.7151,-832.885;Float;False;Property;_BlendTextureMode;Blend Texture Mode;20;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-2164.989,-3356.508;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-92.35077,-1247.51;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;119;123.9022,-429.3918;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;985.5206,-532.7785;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-92.35077,-1376.695;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;296.463,-1594.75;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;75;437.2286,-1906.785;Inherit;True;Property;_Normal;Normal;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;88;287.6019,-1367.028;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;128;994.7536,-418.541;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;167;-1787.788,-3203.71;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1442.579,-1523.725;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;DawnShader_V2/Clothing;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;56;1;43;0
WireConnection;56;2;2;2
WireConnection;57;1;50;0
WireConnection;57;2;2;3
WireConnection;55;1;39;0
WireConnection;55;2;2;1
WireConnection;60;1;1;0
WireConnection;60;0;55;0
WireConnection;62;1;1;0
WireConnection;62;0;57;0
WireConnection;61;1;1;0
WireConnection;61;0;56;0
WireConnection;111;0;79;0
WireConnection;111;1;2;1
WireConnection;112;0;91;0
WireConnection;112;1;2;2
WireConnection;139;0;140;0
WireConnection;139;1;2;3
WireConnection;29;1;10;0
WireConnection;29;2;2;2
WireConnection;66;0;62;0
WireConnection;66;1;2;3
WireConnection;113;0;93;0
WireConnection;113;1;2;3
WireConnection;28;1;8;0
WireConnection;28;2;2;1
WireConnection;145;0;143;0
WireConnection;145;1;2;2
WireConnection;64;0;60;0
WireConnection;64;1;2;1
WireConnection;30;1;13;0
WireConnection;30;2;2;3
WireConnection;150;0;148;0
WireConnection;65;0;61;0
WireConnection;65;1;2;2
WireConnection;138;0;141;0
WireConnection;138;1;2;1
WireConnection;152;0;150;0
WireConnection;152;2;151;0
WireConnection;152;1;149;2
WireConnection;144;0;138;0
WireConnection;144;1;145;0
WireConnection;144;2;139;0
WireConnection;147;0;168;2
WireConnection;121;0;102;0
WireConnection;121;1;2;2
WireConnection;146;0;28;0
WireConnection;146;1;29;0
WireConnection;146;2;30;0
WireConnection;118;0;111;0
WireConnection;118;1;112;0
WireConnection;118;2;113;0
WireConnection;120;0;101;0
WireConnection;120;1;2;1
WireConnection;122;0;104;0
WireConnection;122;1;2;3
WireConnection;27;0;2;1
WireConnection;27;1;2;2
WireConnection;27;2;2;3
WireConnection;27;3;2;4
WireConnection;70;0;64;0
WireConnection;70;1;65;0
WireConnection;70;2;66;0
WireConnection;153;1;152;0
WireConnection;117;0;118;0
WireConnection;117;1;147;0
WireConnection;127;0;120;0
WireConnection;127;1;121;0
WireConnection;127;2;122;0
WireConnection;132;0;1;0
WireConnection;132;1;146;0
WireConnection;132;2;144;0
WireConnection;71;0;1;0
WireConnection;71;1;70;0
WireConnection;71;2;27;0
WireConnection;165;0;155;1
WireConnection;165;1;153;0
WireConnection;165;2;160;0
WireConnection;165;3;157;0
WireConnection;83;0;86;0
WireConnection;83;1;80;4
WireConnection;163;0;155;3
WireConnection;163;1;153;0
WireConnection;163;2;158;0
WireConnection;163;3;162;0
WireConnection;84;0;170;0
WireConnection;84;1;86;0
WireConnection;72;1;132;0
WireConnection;72;0;71;0
WireConnection;164;0;155;2
WireConnection;164;1;153;0
WireConnection;164;2;161;0
WireConnection;164;3;156;0
WireConnection;81;0;86;0
WireConnection;81;1;80;4
WireConnection;119;0;147;0
WireConnection;119;1;117;0
WireConnection;119;2;27;0
WireConnection;126;0;127;0
WireConnection;126;1;168;3
WireConnection;82;0;169;0
WireConnection;82;1;86;0
WireConnection;87;0;72;0
WireConnection;87;1;84;0
WireConnection;87;2;83;0
WireConnection;88;0;119;0
WireConnection;88;1;82;0
WireConnection;88;2;81;0
WireConnection;128;0;168;3
WireConnection;128;1;126;0
WireConnection;128;2;27;0
WireConnection;167;0;165;0
WireConnection;167;1;164;0
WireConnection;167;2;163;0
WireConnection;0;0;87;0
WireConnection;0;1;75;0
WireConnection;0;2;167;0
WireConnection;0;3;128;0
WireConnection;0;4;88;0
WireConnection;0;5;168;1
ASEEND*/
//CHKSM=A0536D660B731A79B0BA4FAAA60641FCE6FE9E0C