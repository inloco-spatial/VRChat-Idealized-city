// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterLeviant"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "Water.hlsl"
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ViewVector1 = ase_worldViewDir;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ViewNormal1 = ase_worldNormal;
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			float3 ViewTangent1 = ase_worldTangent;
			half tangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3 ViewBitangent1 = ase_worldBitangent;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 ScreenPosition1 = ase_screenPosNorm;
			float2 uv01 = float2( 0,0 );
			float2 uv11 = float2( 0,0 );
			float2 foamUV1 = float2( 0,0 );
			float3 GrabTexel1 = float3( 0,0,0 );
			float Depth1 = 0.0;
			float3 Albedo1 = float3( 0,0,0 );
			float Smoothness1 = 0.0;
			float3 Emission1 = float3( 0,0,0 );
			float3 TangentNormal1 = float3( 0,0,0 );
			float3 TangentWorld1 = float3( 0,0,0 );
			float localMyCustomExpression1 = MyCustomExpression( ViewVector1 , ViewNormal1 , ViewTangent1 , ViewBitangent1 , ScreenPosition1 , uv01 , uv11 , foamUV1 , GrabTexel1 , Depth1 , Albedo1 , Smoothness1 , Emission1 , TangentNormal1 , TangentWorld1 );
			v.normal = TangentNormal1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ViewVector1 = ase_worldViewDir;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ViewNormal1 = ase_worldNormal;
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ViewTangent1 = ase_worldTangent;
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3 ViewBitangent1 = ase_worldBitangent;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 ScreenPosition1 = ase_screenPosNorm;
			float2 uv01 = float2( 0,0 );
			float2 uv11 = float2( 0,0 );
			float2 foamUV1 = float2( 0,0 );
			float3 GrabTexel1 = float3( 0,0,0 );
			float Depth1 = 0.0;
			float3 Albedo1 = float3( 0,0,0 );
			float Smoothness1 = 0.0;
			float3 Emission1 = float3( 0,0,0 );
			float3 TangentNormal1 = float3( 0,0,0 );
			float3 TangentWorld1 = float3( 0,0,0 );
			float localMyCustomExpression1 = MyCustomExpression( ViewVector1 , ViewNormal1 , ViewTangent1 , ViewBitangent1 , ScreenPosition1 , uv01 , uv11 , foamUV1 , GrabTexel1 , Depth1 , Albedo1 , Smoothness1 , Emission1 , TangentNormal1 , TangentWorld1 );
			o.Albedo = Albedo1;
			o.Emission = Emission1;
			o.Smoothness = Smoothness1;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float4 screenPos : TEXCOORD1;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
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
Version=19200
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-3.200073,63.19998;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;WaterLeviant;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CustomExpressionNode;1;-589.2648,90.18768;Inherit;False; ;1;File;15;False;ViewVector;FLOAT3;0,0,0;In;;Inherit;False;False;ViewNormal;FLOAT3;0,0,0;In;;Inherit;False;False;ViewTangent;FLOAT3;0,0,0;In;;Inherit;False;False;ViewBitangent;FLOAT3;0,0,0;In;;Inherit;False;False;ScreenPosition;FLOAT4;0,0,0,0;In;;Inherit;False;False;uv0;FLOAT2;0,0;In;;Inherit;False;False;uv1;FLOAT2;0,0;In;;Inherit;False;False;foamUV;FLOAT2;0,0;In;;Inherit;False;False;GrabTexel;FLOAT3;0,0,0;In;;Inherit;False;False;Depth;FLOAT;0;In;;Inherit;False;False;Albedo;FLOAT3;0,0,0;Out;;Inherit;False;False;Smoothness;FLOAT;0;Out;;Inherit;False;False;Emission;FLOAT3;0,0,0;Out;;Inherit;False;False;TangentNormal;FLOAT3;0,0,0;Out;;Inherit;False;False;TangentWorld;FLOAT3;0,0,0;Out;;Inherit;False;My Custom Expression;False;False;0;c406a175a51014645862c50dbc179c92;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT3;0,0,0;False;9;FLOAT;0;False;10;FLOAT3;0,0,0;False;11;FLOAT;0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT3;0,0,0;False;6;FLOAT;0;FLOAT3;11;FLOAT;12;FLOAT3;13;FLOAT3;14;FLOAT3;15
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;2;-1280.467,-230.6125;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexBinormalNode;5;-831.6649,14.1875;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexTangentNode;6;-998.0645,-29.81244;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;7;-1174.865,-76.21246;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScreenPosInputsNode;11;-1214.865,162.1876;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;0;0;1;11
WireConnection;0;2;1;13
WireConnection;0;4;1;12
WireConnection;0;12;1;14
WireConnection;1;0;2;0
WireConnection;1;1;7;0
WireConnection;1;2;6;0
WireConnection;1;3;5;0
WireConnection;1;4;11;0
ASEEND*/
//CHKSM=95624DBBA201130D4F30F1EFD384A7EA29649F8C