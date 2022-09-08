// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:22,stmr:255,stmw:255,stcp:7,stps:3,stfa:0,stfz:4,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32719,y:32712,varname:node_2865,prsc:2|diff-6354-OUT,spec-5248-OUT,gloss-9000-OUT,normal-1487-OUT,difocc-2076-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31593,y:32022,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31731,y:32711,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:358,x:32237,y:32521,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32005,y:32846,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_NormalVector,id:4652,x:30310,y:32007,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:139,x:30490,y:32008,varname:node_139,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-4652-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:3897,x:30303,y:32212,varname:node_3897,prsc:2;n:type:ShaderForge.SFN_Normalize,id:5218,x:30497,y:32201,varname:node_5218,prsc:2|IN-3897-Y;n:type:ShaderForge.SFN_Dot,id:7522,x:30724,y:32134,varname:node_7522,prsc:2,dt:1|A-139-G,B-5218-OUT;n:type:ShaderForge.SFN_Dot,id:9937,x:30709,y:32332,varname:node_9937,prsc:2,dt:2|A-139-G,B-5218-OUT;n:type:ShaderForge.SFN_If,id:4833,x:30947,y:32152,varname:node_4833,prsc:2|A-5218-OUT,B-7241-OUT,GT-7522-OUT,EQ-9937-OUT,LT-9937-OUT;n:type:ShaderForge.SFN_Vector1,id:7241,x:30726,y:32273,varname:node_7241,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:3229,x:31155,y:32092,varname:node_3229,prsc:2|A-139-R,B-4833-OUT,C-139-B;n:type:ShaderForge.SFN_Multiply,id:9363,x:31380,y:32471,varname:node_9363,prsc:2|A-5405-RGB,B-2854-OUT;n:type:ShaderForge.SFN_Multiply,id:1773,x:31351,y:32100,varname:node_1773,prsc:2|A-3229-OUT,B-3229-OUT;n:type:ShaderForge.SFN_Tex2d,id:5405,x:31093,y:32453,ptovrint:True,ptlb:SnowCover,ptin:_Snow,varname:_Snow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:2854,x:31009,y:32691,ptovrint:False,ptlb:SnowAmount,ptin:_SnowAmount,varname:_SnowAmount,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector3,id:6649,x:31359,y:32297,varname:node_6649,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_ChannelBlend,id:6187,x:31623,y:32248,varname:node_6187,prsc:2,chbt:0|M-1773-OUT,R-6649-OUT,G-9363-OUT,B-6649-OUT;n:type:ShaderForge.SFN_Blend,id:6354,x:32142,y:32051,varname:node_6354,prsc:2,blmd:18,clmp:True|SRC-7736-RGB,DST-6187-OUT;n:type:ShaderForge.SFN_NormalVector,id:7126,x:30486,y:32717,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:2921,x:30666,y:32718,varname:node_2921,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-7126-OUT;n:type:ShaderForge.SFN_Normalize,id:9745,x:30673,y:32911,varname:node_9745,prsc:2|IN-9790-Y;n:type:ShaderForge.SFN_FragmentPosition,id:9790,x:30435,y:32906,varname:node_9790,prsc:2;n:type:ShaderForge.SFN_Dot,id:4169,x:30889,y:32853,varname:node_4169,prsc:2,dt:1|A-2921-G,B-9745-OUT;n:type:ShaderForge.SFN_Dot,id:3808,x:30885,y:33042,varname:node_3808,prsc:2,dt:2|A-2921-G,B-9745-OUT;n:type:ShaderForge.SFN_If,id:2734,x:31112,y:32871,varname:node_2734,prsc:2|A-9745-OUT,B-6489-OUT,GT-4169-OUT,EQ-3808-OUT,LT-3808-OUT;n:type:ShaderForge.SFN_Append,id:6535,x:31303,y:32841,varname:node_6535,prsc:2|A-2921-R,B-2734-OUT,C-2921-B;n:type:ShaderForge.SFN_Tex2d,id:300,x:31292,y:33152,ptovrint:True,ptlb:SnowCoverNormal,ptin:_SnowNormal,varname:_SnowNormal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:9169,x:31169,y:33374,ptovrint:False,ptlb:SnowAmountNormal,ptin:_SnowAmountNormal,varname:_SnowAmountNormal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:2680,x:31496,y:33154,varname:node_2680,prsc:2|A-300-RGB,B-9169-OUT;n:type:ShaderForge.SFN_Multiply,id:7408,x:31497,y:32839,varname:node_7408,prsc:2|A-6535-OUT,B-6535-OUT;n:type:ShaderForge.SFN_Vector3,id:6016,x:31500,y:33013,varname:node_6016,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_ChannelBlend,id:3351,x:31739,y:32931,varname:node_3351,prsc:2,chbt:0|M-7408-OUT,R-6016-OUT,G-2680-OUT,B-6016-OUT;n:type:ShaderForge.SFN_Vector1,id:6489,x:30889,y:32997,varname:node_6489,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:1487,x:32091,y:32615,varname:node_1487,prsc:2|A-5964-RGB,B-3351-OUT;n:type:ShaderForge.SFN_Tex2d,id:9538,x:31955,y:32989,ptovrint:False,ptlb:M/AO/G,ptin:_MAOG,varname:_MAOG,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9000,x:32276,y:33083,varname:node_9000,prsc:2|A-9538-A,B-1813-OUT;n:type:ShaderForge.SFN_Multiply,id:5248,x:32605,y:32457,varname:node_5248,prsc:2|A-9538-R,B-358-OUT;n:type:ShaderForge.SFN_Slider,id:6599,x:31857,y:33293,ptovrint:False,ptlb:Ao,ptin:_Ao,varname:_Ao,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:2076,x:32386,y:33328,varname:node_2076,prsc:2|A-9538-G,B-6599-OUT;n:type:ShaderForge.SFN_Clamp01,id:8464,x:30958,y:29437,varname:node_8464,prsc:2|IN-5849-OUT;n:type:ShaderForge.SFN_Power,id:5849,x:30780,y:29436,varname:node_5849,prsc:2|VAL-127-OUT,EXP-5748-OUT;n:type:ShaderForge.SFN_Multiply,id:127,x:30582,y:29441,varname:node_127,prsc:2|B-5261-OUT;n:type:ShaderForge.SFN_Append,id:2833,x:30153,y:29422,varname:node_2833,prsc:2|A-4526-OUT,B-2614-OUT,C-5103-A;n:type:ShaderForge.SFN_NormalVector,id:3910,x:30147,y:29584,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2614,x:29754,y:29569,ptovrint:False,ptlb:UpNode_copy,ptin:_UpNode_copy,varname:_UpNode_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:5261,x:29822,y:29778,ptovrint:False,ptlb:Levels_copy,ptin:_Levels_copy,varname:_Levels_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:5748,x:29917,y:29974,ptovrint:False,ptlb:Contrast_copy,ptin:_Contrast_copy,varname:_Contrast_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_ValueProperty,id:4526,x:29813,y:29374,ptovrint:False,ptlb:Value_copy,ptin:_Value_copy,varname:_Value_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:5103,x:29567,y:29400,ptovrint:False,ptlb:SpecularMap_copy,ptin:_SpecularMap_copy,varname:_SpecularMap_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Clamp01,id:8093,x:32009,y:31670,varname:node_8093,prsc:2|IN-7736-RGB;proporder:358-1813-7736-5964-9538-6599-5405-300-2854-9169;pass:END;sub:END;*/

Shader "TobyFredson/SnowCoverShader" {
    Properties {
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
        _MainTex ("Base Color", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MAOG ("M/AO/G", 2D) = "white" {}
        _Ao ("Ao", Range(0, 1)) = 0
        _Snow ("SnowCover", 2D) = "white" {}
        _SnowNormal ("SnowCoverNormal", 2D) = "bump" {}
        _SnowAmount ("SnowAmount", Range(0, 1)) = 0
        _SnowAmountNormal ("SnowAmountNormal", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Snow; uniform float4 _Snow_ST;
            uniform float _SnowAmount;
            uniform sampler2D _SnowNormal; uniform float4 _SnowNormal_ST;
            uniform float _SnowAmountNormal;
            uniform sampler2D _MAOG; uniform float4 _MAOG_ST;
            uniform float _Ao;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 node_2921 = i.normalDir.rgb;
                float node_9745 = normalize(i.posWorld.g);
                float node_2734_if_leA = step(node_9745,0.0);
                float node_2734_if_leB = step(0.0,node_9745);
                float node_3808 = min(0,dot(node_2921.g,node_9745));
                float3 node_6535 = float3(node_2921.r,lerp((node_2734_if_leA*node_3808)+(node_2734_if_leB*max(0,dot(node_2921.g,node_9745))),node_3808,node_2734_if_leA*node_2734_if_leB),node_2921.b);
                float3 node_7408 = (node_6535*node_6535);
                float3 node_6016 = float3(0,0,0);
                float3 _SnowNormal_var = UnpackNormal(tex2D(_SnowNormal,TRANSFORM_TEX(i.uv0, _SnowNormal)));
                float3 normalLocal = (_BumpMap_var.rgb+(node_7408.r*node_6016 + node_7408.g*(_SnowNormal_var.rgb*_SnowAmountNormal) + node_7408.b*node_6016));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MAOG_var = tex2D(_MAOG,TRANSFORM_TEX(i.uv0, _MAOG));
                float gloss = (_MAOG_var.a*_Gloss);
                float perceptualRoughness = 1.0 - (_MAOG_var.a*_Gloss);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = (_MAOG_var.r*_Metallic);
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_139 = i.normalDir.rgb;
                float node_5218 = normalize(i.posWorld.g);
                float node_4833_if_leA = step(node_5218,0.0);
                float node_4833_if_leB = step(0.0,node_5218);
                float node_9937 = min(0,dot(node_139.g,node_5218));
                float3 node_3229 = float3(node_139.r,lerp((node_4833_if_leA*node_9937)+(node_4833_if_leB*max(0,dot(node_139.g,node_5218))),node_9937,node_4833_if_leA*node_4833_if_leB),node_139.b);
                float3 node_1773 = (node_3229*node_3229);
                float3 node_6649 = float3(0,0,0);
                float4 _Snow_var = tex2D(_Snow,TRANSFORM_TEX(i.uv0, _Snow));
                float3 diffuseColor = saturate((0.5 - 2.0*(_MainTex_var.rgb-0.5)*((node_1773.r*node_6649 + node_1773.g*(_Snow_var.rgb*_SnowAmount) + node_1773.b*node_6649)-0.5))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= (_MAOG_var.g+_Ao); // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Snow; uniform float4 _Snow_ST;
            uniform float _SnowAmount;
            uniform sampler2D _SnowNormal; uniform float4 _SnowNormal_ST;
            uniform float _SnowAmountNormal;
            uniform sampler2D _MAOG; uniform float4 _MAOG_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 node_2921 = i.normalDir.rgb;
                float node_9745 = normalize(i.posWorld.g);
                float node_2734_if_leA = step(node_9745,0.0);
                float node_2734_if_leB = step(0.0,node_9745);
                float node_3808 = min(0,dot(node_2921.g,node_9745));
                float3 node_6535 = float3(node_2921.r,lerp((node_2734_if_leA*node_3808)+(node_2734_if_leB*max(0,dot(node_2921.g,node_9745))),node_3808,node_2734_if_leA*node_2734_if_leB),node_2921.b);
                float3 node_7408 = (node_6535*node_6535);
                float3 node_6016 = float3(0,0,0);
                float3 _SnowNormal_var = UnpackNormal(tex2D(_SnowNormal,TRANSFORM_TEX(i.uv0, _SnowNormal)));
                float3 normalLocal = (_BumpMap_var.rgb+(node_7408.r*node_6016 + node_7408.g*(_SnowNormal_var.rgb*_SnowAmountNormal) + node_7408.b*node_6016));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MAOG_var = tex2D(_MAOG,TRANSFORM_TEX(i.uv0, _MAOG));
                float gloss = (_MAOG_var.a*_Gloss);
                float perceptualRoughness = 1.0 - (_MAOG_var.a*_Gloss);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = (_MAOG_var.r*_Metallic);
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_139 = i.normalDir.rgb;
                float node_5218 = normalize(i.posWorld.g);
                float node_4833_if_leA = step(node_5218,0.0);
                float node_4833_if_leB = step(0.0,node_5218);
                float node_9937 = min(0,dot(node_139.g,node_5218));
                float3 node_3229 = float3(node_139.r,lerp((node_4833_if_leA*node_9937)+(node_4833_if_leB*max(0,dot(node_139.g,node_5218))),node_9937,node_4833_if_leA*node_4833_if_leB),node_139.b);
                float3 node_1773 = (node_3229*node_3229);
                float3 node_6649 = float3(0,0,0);
                float4 _Snow_var = tex2D(_Snow,TRANSFORM_TEX(i.uv0, _Snow));
                float3 diffuseColor = saturate((0.5 - 2.0*(_MainTex_var.rgb-0.5)*((node_1773.r*node_6649 + node_1773.g*(_Snow_var.rgb*_SnowAmount) + node_1773.b*node_6649)-0.5))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _Snow; uniform float4 _Snow_ST;
            uniform float _SnowAmount;
            uniform sampler2D _MAOG; uniform float4 _MAOG_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_139 = i.normalDir.rgb;
                float node_5218 = normalize(i.posWorld.g);
                float node_4833_if_leA = step(node_5218,0.0);
                float node_4833_if_leB = step(0.0,node_5218);
                float node_9937 = min(0,dot(node_139.g,node_5218));
                float3 node_3229 = float3(node_139.r,lerp((node_4833_if_leA*node_9937)+(node_4833_if_leB*max(0,dot(node_139.g,node_5218))),node_9937,node_4833_if_leA*node_4833_if_leB),node_139.b);
                float3 node_1773 = (node_3229*node_3229);
                float3 node_6649 = float3(0,0,0);
                float4 _Snow_var = tex2D(_Snow,TRANSFORM_TEX(i.uv0, _Snow));
                float3 diffColor = saturate((0.5 - 2.0*(_MainTex_var.rgb-0.5)*((node_1773.r*node_6649 + node_1773.g*(_Snow_var.rgb*_SnowAmount) + node_1773.b*node_6649)-0.5)));
                float specularMonochrome;
                float3 specColor;
                float4 _MAOG_var = tex2D(_MAOG,TRANSFORM_TEX(i.uv0, _MAOG));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, (_MAOG_var.r*_Metallic), specColor, specularMonochrome );
                float roughness = 1.0 - (_MAOG_var.a*_Gloss);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
