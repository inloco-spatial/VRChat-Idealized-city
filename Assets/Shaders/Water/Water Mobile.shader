Shader "Leviant's Shaders/Water Mobile"
{
	Properties
	{
		_DeepColor ("Deep Color", Color) = (0.04, 0.1, 0.27, 0.7)
		_ShallowColor ("Shallow Color", Color) = (0.04, 0.55, 0.55, 0.2)
		_WaterDepth ("Depth", Float) = 0.3
		_Displacement ("Displacement", Float) = 10.0
		_DisplacementLOD ("DisplacementLOD", Range(0, 14)) = 2.0
		_Distortion ("Distortion Power", Float) = 0.25
		//_ScreenDistortion ("ScreenDistortion", Float) = 0.0
		//_Blur ("Blur", Float) = 1.5
		//[IntRange] _Samples ("Samples", Range(1, 64)) = 8.0
		_NormalTex ("Main Normal", 2D) = "bump" {}
		_FlowSpeed ("FlowSpeed", Vector) = (0.01, 0.0095, 0, 0)
		[Toggle(USE_FOAM)]UseFoam ("UseFoam", Int) = 1
		_ShorelineWidth ("ShorelineWidth", Float) = 0.5
		_Shoreline_Wave_Speed ("Shoreline Wave Speed", Float) = 0.15
		_FoamTex ("Foam Texture", 2D) = "white" {}
		_Foam ("Foam", Float) = 10.0
		_FoamOffset ("FoamOffset", Float) = -0.12
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade
		#pragma target 4.0
		#define ALPHABLEND_ON
		#include "Water.cginc"
		ENDCG
	}
}
