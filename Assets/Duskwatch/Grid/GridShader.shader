// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GridShader"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_GridTex("GridTex", 2D) = "white" {}
		_ObjectPlacement("ObjectPlacement", 2D) = "white" {}
		_LineThickness1("LineThickness", Range( 0 , 1)) = 0.1
		_GapThickness1("GapThickness", Range( 0 , 1)) = 0.1
		_GridSize("GridSize", Float) = 512
		_FadeDistance("FadeDistance", Float) = 10
		_DefaultColour("DefaultColour", Color) = (0.8196079,0.8470588,0.8784314,1)
		_ValidColour("ValidColour", Color) = (0.9960784,0.827451,0.1882353,1)
		_InvalidColour("InvalidColour", Color) = (0.9215686,0.2313726,0.3529412,1)
		_Position("Position", Vector) = (0,0,0,0)
		[Toggle(_SHOWGRID_ON)] _ShowGrid("ShowGrid", Float) = 0
		_PlacementGridAlpha("PlacementGridAlpha", Range( 0 , 1)) = 0.5
		_GridAlpha("GridAlpha", Range( 0 , 1)) = 0.2


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull Back
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _SHOWGRID_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD2;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD3;
				#endif
				#if defined(LIGHTMAP_ON)
					float4 lightmapUVOrVertexSH : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;
			sampler2D _GridTex;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord6.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord6.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				float3 WorldPosition = input.positionWS;
				float3 WorldViewDirection = GetWorldSpaceNormalizeViewDir( WorldPosition );
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 appendResult16 = (float2(WorldPosition.x , WorldPosition.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float4 lerpResult90 = lerp( _DefaultColour , _ValidColour , ObjPlacementMap89);
				float InvalidMap79 = round( tex2D( _GridTex, WorldUV101 ).r );
				float4 lerpResult81 = lerp( lerpResult90 , _InvalidColour , InvalidMap79);
				float4 Colour82 = lerpResult81;
				
				float GridSize12 = _GridSize;
				float2 temp_cast_1 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_2 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord6.xy*temp_cast_1 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_2 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_3 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_4 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord6.xy*temp_cast_3 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_4 ) ) / Radius25_g9 );
				float2 temp_cast_5 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_6 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord6.xy*temp_cast_5 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_6 ) ) / Radius25_g11 );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = Colour82.rgb;
				float Alpha = staticSwitch54;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(input.positionCS, Color);
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						Color.rgb = MixFogColor(Color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						Color.rgb = MixFog(Color.rgb, inputData.fogCoord);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _SHOWGRID_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;


			
			float3 _LightDirection;
			float3 _LightPosition;

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				//code for UNITY_REVERSED_Z is moved into Shadows.hlsl from 6000.0.22 and or higher
				positionCS = ApplyShadowClamping(positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = positionCS;
				output.clipPosV = positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float2 appendResult16 = (float2(WorldPosition.x , WorldPosition.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				float Alpha = staticSwitch54;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _SHOWGRID_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float2 appendResult16 = (float2(WorldPosition.x , WorldPosition.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				float Alpha = staticSwitch54;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma shader_feature_local _SHOWGRID_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				output.ase_texcoord1.xyz = ase_positionWS;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float3 ase_positionWS = input.ase_texcoord1.xyz;
				float2 appendResult16 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch54;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#pragma shader_feature_local _SHOWGRID_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				output.ase_texcoord1.xyz = ase_positionWS;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float3 ase_positionWS = input.ase_texcoord1.xyz;
				float2 appendResult16 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch54;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

        	#pragma multi_compile_instancing
        	#pragma multi_compile _ LOD_FADE_CROSSFADE
        	#define ASE_FOG 1
        	#define _SURFACE_TYPE_TRANSPARENT 1
        	#define ASE_VERSION 19801
        	#define ASE_SRP_VERSION 170003


        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _SHOWGRID_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _ObjectPlacement;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = TransformObjectToWorldNormal( input.normalOS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );
				float3 WorldPosition = input.positionWS;
				float3 WorldNormal = input.normalWS;
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord3.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float2 appendResult16 = (float2(WorldPosition.x , WorldPosition.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				float Alpha = staticSwitch54;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(input.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = input.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			ColorMask RG

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
		    #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

			#pragma shader_feature_local _SHOWGRID_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DefaultColour;
			float4 _ValidColour;
			float4 _InvalidColour;
			float2 _Position;
			float _GridSize;
			float _GapThickness1;
			float _LineThickness1;
			float _GridAlpha;
			float _PlacementGridAlpha;
			float _FadeDistance;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _ObjectPlacement;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				output.ase_texcoord3.xyz = ase_positionWS;
				
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				output.ase_texcoord3.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				// Jittered. Match the frame.
				output.positionCS = vertexInput.positionCS;
				output.positionCSNoJitter = mul( _NonJitteredViewProjMatrix, mul( UNITY_MATRIX_M, input.positionOS ) );

				float4 prevPos = ( unity_MotionVectorsParams.x == 1 ) ? float4( input.positionOld, 1 ) : input.positionOS;

				#if _ADD_PRECOMPUTED_VELOCITY
					prevPos = prevPos - float4(input.alembicMotionVector, 0);
				#endif

				output.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, mul( UNITY_PREV_MATRIX_M, prevPos ) );

				return output;
			}

			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}

			half4 frag(	PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float GridSize12 = _GridSize;
				float2 temp_cast_0 = (GridSize12).xx;
				float temp_output_28_0 = ( 1.0 - _GapThickness1 );
				float temp_output_7_0_g12 = temp_output_28_0;
				float temp_output_2_0_g13 = temp_output_7_0_g12;
				float temp_output_3_0_g13 = temp_output_7_0_g12;
				float2 appendResult21_g13 = (float2(temp_output_2_0_g13 , temp_output_3_0_g13));
				float Radius25_g13 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g13 ) ) , abs( temp_output_3_0_g13 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g13 = ( length( max( ( ( abs( (frac( (input.ase_texcoord2.xy*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g13 ) + Radius25_g13 ) , temp_cast_1 ) ) / Radius25_g13 );
				float clampResult66 = clamp( saturate( ( ( 1.0 - temp_output_30_0_g13 ) / fwidth( temp_output_30_0_g13 ) ) ) , 0.0 , 0.3 );
				float2 temp_cast_2 = (GridSize12).xx;
				float temp_output_7_0_g8 = temp_output_28_0;
				float temp_output_2_0_g9 = temp_output_7_0_g8;
				float temp_output_3_0_g9 = temp_output_7_0_g8;
				float2 appendResult21_g9 = (float2(temp_output_2_0_g9 , temp_output_3_0_g9));
				float Radius25_g9 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g9 ) ) , abs( temp_output_3_0_g9 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g9 = ( length( max( ( ( abs( (frac( (input.ase_texcoord2.xy*temp_cast_2 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g9 ) + Radius25_g9 ) , temp_cast_3 ) ) / Radius25_g9 );
				float2 temp_cast_4 = (GridSize12).xx;
				float temp_output_7_0_g10 = ( temp_output_28_0 - _LineThickness1 );
				float temp_output_2_0_g11 = temp_output_7_0_g10;
				float temp_output_3_0_g11 = temp_output_7_0_g10;
				float2 appendResult21_g11 = (float2(temp_output_2_0_g11 , temp_output_3_0_g11));
				float Radius25_g11 = max( min( min( abs( ( 0.2 * 2 ) ) , abs( temp_output_2_0_g11 ) ) , abs( temp_output_3_0_g11 ) ) , 1E-05 );
				float2 temp_cast_5 = (0.0).xx;
				float temp_output_30_0_g11 = ( length( max( ( ( abs( (frac( (input.ase_texcoord2.xy*temp_cast_4 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult21_g11 ) + Radius25_g11 ) , temp_cast_5 ) ) / Radius25_g11 );
				float3 ase_positionWS = input.ase_texcoord3.xyz;
				float2 appendResult16 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 WorldPos17 = appendResult16;
				float2 WorldUV101 = ( ( floor( ( WorldPos17 / 2.0 ) ) / 256.0 ) + float2( 0.5,0.5 ) );
				float ObjPlacementMap89 = round( tex2D( _ObjectPlacement, WorldUV101 ).r );
				float lerpResult125 = lerp( _GridAlpha , _PlacementGridAlpha , ObjPlacementMap89);
				float Grid46 = (0.0 + (saturate( ( clampResult66 + saturate( ( saturate( ( ( 1.0 - temp_output_30_0_g9 ) / fwidth( temp_output_30_0_g9 ) ) ) - saturate( ( ( 1.0 - temp_output_30_0_g11 ) / fwidth( temp_output_30_0_g11 ) ) ) ) ) ) ) - 0.0) * (lerpResult125 - 0.0) / (1.0 - 0.0));
				float smoothstepResult24 = smoothstep( 0.0 , 1.0 , ( 1.0 - saturate( ( ( -5.0 + distance( WorldPos17 , _Position ) ) / _FadeDistance ) ) ));
				float DistanceAlpha25 = smoothstepResult24;
				#ifdef _SHOWGRID_ON
				float staticSwitch54 = ( Grid46 * DistanceAlpha25 );
				#else
				float staticSwitch54 = 0.0;
				#endif
				

				float Alpha = staticSwitch54;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( input.positionCS );
				#endif

				return float4( CalcNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 0, 0 );
			}
			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.WorldPosInputsNode;15;-4144,-2464;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;16;-3952,-2432;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-3776,-2432;Inherit;False;WorldPos;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2976,-2096;Inherit;False;17;WorldPos;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2944,-1952;Inherit;False;Constant;_WorldScale;WorldScale;9;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;95;-2752,-2096;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2656,-1952;Inherit;False;Constant;_MapSize;MapSize;9;0;Create;True;0;0;0;False;0;False;256;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;97;-2592,-2096;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;98;-2448,-1936;Inherit;False;Constant;_MapUVOffset;MapUVOffset;9;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;99;-2448,-2096;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2774.324,1688.343;Inherit;False;Property;_GapThickness1;GapThickness;3;0;Create;True;0;0;0;False;0;False;0.1;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2240,-2480;Inherit;False;Property;_GridSize;GridSize;4;0;Create;True;0;0;0;False;0;False;512;512;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-2224,-2016;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;28;-2470.324,1688.343;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2774.324,1896.343;Inherit;False;Property;_LineThickness1;LineThickness;2;0;Create;True;0;0;0;False;0;False;0.1;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2080,-2480;Inherit;False;GridSize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-2096,-2016;Inherit;False;WorldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-2326.324,1896.343;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-2374.324,1512.343;Inherit;False;12;GridSize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;18;-3856,-2304;Inherit;False;Property;_Position;Position;9;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;87;-2640,-864;Inherit;True;Property;_ObjectPlacement;ObjectPlacement;1;0;Create;True;0;0;0;False;0;False;ebe11718211e41e4d9c80bbb654f9351;ebe11718211e41e4d9c80bbb654f9351;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;86;-2608,-656;Inherit;True;101;WorldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;32;-2070.324,1864.343;Inherit;False;RoundedGrid;-1;;8;f6811bffe01d39a44a9571464ff66648;0;4;4;FLOAT2;8,8;False;3;FLOAT2;0,0;False;7;FLOAT;0.9;False;8;FLOAT;0.2;False;1;FLOAT;9
Node;AmplifyShaderEditor.FunctionNode;33;-2070.324,2008.343;Inherit;False;RoundedGrid;-1;;10;f6811bffe01d39a44a9571464ff66648;0;4;4;FLOAT2;8,8;False;3;FLOAT2;0,0;False;7;FLOAT;0.9;False;8;FLOAT;0.2;False;1;FLOAT;9
Node;AmplifyShaderEditor.DistanceOpNode;20;-3536,-2400;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3424,-2544;Inherit;False;Constant;_FullDistance;FullDistance;6;0;Create;True;0;0;0;False;0;False;-5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;88;-2336,-688;Inherit;True;Property;_TextureSample2;Texture Sample 2;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleSubtractOpNode;35;-1814.324,1960.343;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;47;-1942.324,1544.343;Inherit;False;RoundedGrid;-1;;12;f6811bffe01d39a44a9571464ff66648;0;4;4;FLOAT2;8,8;False;3;FLOAT2;0,0;False;7;FLOAT;0.9;False;8;FLOAT;0.2;False;1;FLOAT;9
Node;AmplifyShaderEditor.RangedFloatNode;64;-1904,1760;Inherit;False;Constant;_CellAlpha;CellAlpha;6;0;Create;True;0;0;0;False;0;False;0.3;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-3200,-2272;Inherit;False;Property;_FadeDistance;FadeDistance;5;0;Create;True;0;0;0;False;0;False;10;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-3168,-2384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;92;-2000,-640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;36;-1638.324,1960.343;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;66;-1538.112,1679.967;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;52;-2896,-2352;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;89;-1792,-656;Inherit;True;ObjPlacementMap;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1334.324,1752.343;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;-2688,-2352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;-1664,2368;Inherit;False;89;ObjPlacementMap;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1648,2208;Inherit;False;Property;_PlacementGridAlpha;PlacementGridAlpha;11;0;Create;True;0;0;0;False;0;False;0.5;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-1648,2128;Inherit;False;Property;_GridAlpha;GridAlpha;12;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-1206.324,1752.343;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;-2528,-2352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;125;-1237.731,2110.556;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-1046.324,1752.343;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;-2336,-2352;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-854.3242,1752.343;Inherit;False;Grid;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-2080,-2352;Inherit;False;DistanceAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-704,272;Inherit;False;25;DistanceAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-704,160;Inherit;False;46;Grid;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-464,96;Inherit;False;Constant;_OffAlpha;OffAlpha;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-464,192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;564.8113,-1562.581;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;880,-1552;Inherit;False;Colour;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-264.6204,-101.5168;Inherit;False;82;Colour;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;77;-2304,-1200;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;80;208,-1312;Inherit;True;79;InvalidMap;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;85;-2000,-1184;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-1808,-1200;Inherit;True;InvalidMap;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;90;288,-1744;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-128,-1680;Inherit;False;89;ObjPlacementMap;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;74;32,-1520;Inherit;False;Property;_InvalidColour;InvalidColour;8;0;Create;True;0;0;0;False;0;False;0.9215686,0.2313726,0.3529412,1;0.9215686,0.2313726,0.3529412,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;75;-144,-2112;Inherit;False;Property;_DefaultColour;DefaultColour;6;0;Create;True;0;0;0;False;0;False;0.8196079,0.8470588,0.8784314,1;0.8196079,0.8470588,0.8784314,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;73;-128,-1920;Inherit;False;Property;_ValidColour;ValidColour;7;0;Create;True;0;0;0;False;0;False;0.9960784,0.827451,0.1882353,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;78;-2608,-1072;Inherit;False;101;WorldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;76;-2640,-1280;Inherit;True;Property;_GridTex;GridTex;0;0;Create;True;0;0;0;False;0;False;ced0b0aca21121b479b6486d3fc01800;ced0b0aca21121b479b6486d3fc01800;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.StaticSwitch;54;-288,96;Inherit;False;Property;_ShowGrid;ShowGrid;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;113;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;114;0,0;Float;False;True;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;GridShader;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;9;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;7;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;27;Surface;1;638806613733430991;  Blend;0;0;Two Sided;1;0;Alpha Clipping;0;638806617565484277;  Use Shadow Threshold;0;0;Forward Only;0;0;Cast Shadows;1;0;Receive Shadows;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;11;False;True;True;True;False;False;True;True;True;False;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;115;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;116;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;117;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;118;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;119;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;120;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;121;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;122;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;123;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;16;0;15;1
WireConnection;16;1;15;3
WireConnection;17;0;16;0
WireConnection;95;0;93;0
WireConnection;95;1;94;0
WireConnection;97;0;95;0
WireConnection;99;0;97;0
WireConnection;99;1;96;0
WireConnection;100;0;99;0
WireConnection;100;1;98;0
WireConnection;28;0;27;0
WireConnection;12;0;11;0
WireConnection;101;0;100;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;32;4;31;0
WireConnection;32;7;28;0
WireConnection;33;4;31;0
WireConnection;33;7;30;0
WireConnection;20;0;17;0
WireConnection;20;1;18;0
WireConnection;88;0;87;0
WireConnection;88;1;86;0
WireConnection;35;0;32;9
WireConnection;35;1;33;9
WireConnection;47;4;31;0
WireConnection;47;7;28;0
WireConnection;63;0;56;0
WireConnection;63;1;20;0
WireConnection;92;0;88;1
WireConnection;36;0;35;0
WireConnection;66;0;47;9
WireConnection;66;2;64;0
WireConnection;52;0;63;0
WireConnection;52;1;53;0
WireConnection;89;0;92;0
WireConnection;40;0;66;0
WireConnection;40;1;36;0
WireConnection;22;0;52;0
WireConnection;43;0;40;0
WireConnection;23;0;22;0
WireConnection;125;0;126;0
WireConnection;125;1;38;0
WireConnection;125;2;124;0
WireConnection;45;0;43;0
WireConnection;45;4;125;0
WireConnection;24;0;23;0
WireConnection;46;0;45;0
WireConnection;25;0;24;0
WireConnection;50;0;48;0
WireConnection;50;1;49;0
WireConnection;81;0;90;0
WireConnection;81;1;74;0
WireConnection;81;2;80;0
WireConnection;82;0;81;0
WireConnection;77;0;76;0
WireConnection;77;1;78;0
WireConnection;85;0;77;1
WireConnection;79;0;85;0
WireConnection;90;0;75;0
WireConnection;90;1;73;0
WireConnection;90;2;91;0
WireConnection;54;1;55;0
WireConnection;54;0;50;0
WireConnection;114;2;83;0
WireConnection;114;3;54;0
ASEEND*/
//CHKSM=723E920CB896BAA527693A4A8FC88458B1299CE3