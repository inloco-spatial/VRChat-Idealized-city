// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SeaSand"
{
	Properties
	{
		_wavysand_albedo("wavy-sand_albedo", 2D) = "white" {}
		_SandTiling("SandTiling", Float) = 1
		_WaterDarkHeight("WaterDarkHeight", Float) = 1
		_WaterDarkPow("WaterDarkPow", Float) = 1
		_WaterDarkMul("WaterDarkMul", Float) = 1
		_Met("Met", Range( 0 , 1)) = 1
		_Smooth("Smooth", Range( 0 , 1)) = 1
		[Normal]_wavysand_normalogl("wavy-sand_normal-ogl", 2D) = "bump" {}
		_NormalScale("NormalScale", Range( 0 , 1)) = 1
		_Shadow("Shadow", Range( 0 , 1)) = 1
		_wavysand_ao("wavy-sand_ao", 2D) = "white" {}
		_AO("AO", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform sampler2D _wavysand_normalogl;
		uniform float _SandTiling;
		uniform float _NormalScale;
		uniform sampler2D _wavysand_albedo;
		uniform float _WaterDarkMul;
		uniform float _WaterDarkHeight;
		uniform float _WaterDarkPow;
		uniform float _Shadow;
		uniform float _Met;
		uniform float _Smooth;
		uniform sampler2D _wavysand_ao;
		uniform float _AO;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 break7 = ase_worldPos;
			float4 appendResult6 = (float4(break7.x , break7.z , 0.0 , 0.0));
			float4 temp_output_11_0 = ( appendResult6 * _SandTiling );
			o.Normal = UnpackScaleNormal( tex2D( _wavysand_normalogl, temp_output_11_0.xy ), _NormalScale );
			float4 tex2DNode1 = tex2D( _wavysand_albedo, temp_output_11_0.xy );
			float3 hsvTorgb25 = RGBToHSV( tex2DNode1.rgb );
			float clampResult20 = clamp( ( ( break7.y * _WaterDarkMul ) + _WaterDarkHeight ) , 0.0 , 1.0 );
			float3 hsvTorgb26 = HSVToRGB( float3(hsvTorgb25.x,pow( ( 1.0 - clampResult20 ) , _WaterDarkPow ),hsvTorgb25.z) );
			o.Albedo = hsvTorgb26;
			o.Emission = ( tex2DNode1 * _Shadow ).rgb;
			o.Metallic = _Met;
			o.Smoothness = _Smooth;
			o.Occlusion = ( tex2D( _wavysand_ao, temp_output_11_0.xy ) * _AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-560.7004,152.9212;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;13;-1433.068,500.7745;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldToObjectTransfNode;5;-1473.557,233.2889;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;7;-1136.964,361.6086;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;6;-775.0906,168.6207;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-659.1674,327.0967;Inherit;False;Property;_SandTiling;SandTiling;1;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-294.4998,-111.8;Inherit;True;Property;_wavysand_albedo;wavy-sand_albedo;0;0;Create;True;0;0;0;False;0;False;-1;3592c98159547a54b95cab7601d6172d;3592c98159547a54b95cab7601d6172d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;25;3.44564,-263.3805;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;19;-912.2042,748.8445;Inherit;False;Property;_WaterDarkHeight;WaterDarkHeight;2;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;20;-557.7645,582.1917;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-724.5684,574.489;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-163.661,348.8574;Inherit;True;Property;_wavysand_ao;wavy-sand_ao;10;0;Create;True;0;0;0;False;0;False;-1;067067a8bf7cd7e419d9d323748477a6;067067a8bf7cd7e419d9d323748477a6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;27;-385.3082,618.7869;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-182.6686,154.845;Inherit;True;Property;_wavysand_normalogl;wavy-sand_normal-ogl;7;1;[Normal];Create;True;0;0;0;False;0;False;-1;f404f4f0c710a1d4299b4d1cd831426e;f404f4f0c710a1d4299b4d1cd831426e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;21;277.0959,-396.092;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;100.6584,-372.6846;Inherit;False;Property;_WaterDarkPow;WaterDarkPow;3;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;26;413.0356,-218.2262;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1020.802,568.4477;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1266.213,598.2346;Inherit;False;Property;_WaterDarkMul;WaterDarkMul;4;0;Create;True;0;0;0;True;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;457.4925,307.4222;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;804.063,1.279384;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SeaSand;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;17;500.4019,167.0881;Inherit;False;Property;_Smooth;Smooth;6;0;Create;True;0;0;0;True;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;191.3815,505.7089;Inherit;False;Property;_AO;AO;11;0;Create;True;0;0;0;True;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;261.6173,-27.32173;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.4716981;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;501.4833,88.47051;Inherit;False;Property;_Met;Met;5;0;Create;True;0;0;0;True;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-476.1457,239.576;Inherit;False;Property;_NormalScale;NormalScale;8;0;Create;True;0;0;0;True;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.91437,45.9425;Inherit;False;Property;_Shadow;Shadow;9;0;Create;True;0;0;0;True;0;False;1;1;0;1;0;1;FLOAT;0
WireConnection;11;0;6;0
WireConnection;11;1;12;0
WireConnection;7;0;13;0
WireConnection;6;0;7;0
WireConnection;6;1;7;2
WireConnection;1;1;11;0
WireConnection;25;0;1;0
WireConnection;20;0;18;0
WireConnection;18;0;24;0
WireConnection;18;1;19;0
WireConnection;15;1;11;0
WireConnection;27;0;20;0
WireConnection;14;1;11;0
WireConnection;14;5;23;0
WireConnection;21;0;27;0
WireConnection;21;1;22;0
WireConnection;26;0;25;1
WireConnection;26;1;21;0
WireConnection;26;2;25;3
WireConnection;24;0;7;1
WireConnection;24;1;30;0
WireConnection;31;0;15;0
WireConnection;31;1;32;0
WireConnection;0;0;26;0
WireConnection;0;1;14;0
WireConnection;0;2;33;0
WireConnection;0;3;16;0
WireConnection;0;4;17;0
WireConnection;0;5;31;0
WireConnection;33;0;1;0
WireConnection;33;1;34;0
ASEEND*/
//CHKSM=8DD88BDB602B674F3F51681C46DA2B85CC6D74C0