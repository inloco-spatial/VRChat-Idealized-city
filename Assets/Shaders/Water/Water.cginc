#pragma once
#include "UnityCG.cginc"

#ifdef REFRACTION_ON
uniform half _ScreenDistortion;
uniform half _Blur;
uniform half _Samples;
#endif

uniform half4 _DeepColor;
uniform half4 _ShallowColor;
uniform half _WaterDepth;
uniform half _Displacement;
uniform half _DisplacementLOD;
uniform half _Distortion;
UNITY_DECLARE_TEX2D(_NormalTex);
uniform half4 _NormalTex_ST;
uniform float2 _FlowSpeed;
uniform half _ShorelineWidth;
uniform half _Shoreline_Wave_Speed;
UNITY_DECLARE_TEX2D(_FoamTex);
uniform half4 _FoamTex_ST;
uniform int UseFoam;
uniform half _Foam;
uniform half _FoamOffset;

struct Input
{
	float2 tex;
	float4 grabPos;
	float3 viewTangent;
	float3 viewBitangent;
	float3 viewNormal;
	float3 viewPos;
	//INTERNAL_DATA
};

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_INSTANCING_BUFFER_END(Props)
		
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
UNITY_DECLARE_TEX2DARRAY(_CameraDepthTexture);
UNITY_DECLARE_TEX2DARRAY(_CameraOpaqueTexture);
#else
UNITY_DECLARE_TEX2D(_CameraDepthTexture);
UNITY_DECLARE_TEX2D(_CameraOpaqueTexture);
#endif

half3 SampleSceneColor(half2 uv)
{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
	return UNITY_SAMPLE_TEX2DARRAY(_CameraOpaqueTexture, float3(uv, unity_StereoEyeIndex)).rgb;
#else
	return UNITY_SAMPLE_TEX2D(_CameraOpaqueTexture, uv).rgb;
#endif
}

half SampleSceneDepth(half2 uv)
{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
	return UNITY_SAMPLE_TEX2DARRAY(_CameraDepthTexture, float3(uv, unity_StereoEyeIndex)).r;
#else
	return UNITY_SAMPLE_TEX2D(_CameraDepthTexture, uv).r;
#endif
}

uniform half4 _CameraOpaqueTexture_TexelSize;

void WaterVert_half(inout appdata_full v)
{
	float2 uv0 = _Time.y * _FlowSpeed + TRANSFORM_TEX(v.texcoord, _NormalTex);
	float2 uv1 = float2(uv0.y, -uv0.x);
	
	half4 packedNormal = UNITY_SAMPLE_TEX2D_LOD(_NormalTex, uv0, _DisplacementLOD);
	packedNormal += UNITY_SAMPLE_TEX2D_LOD(_NormalTex, uv1, _DisplacementLOD);
	half3 normal = UnpackNormal(packedNormal / 2.0);
	half height = 1.0 - sqrt(1.0 - dot(normal.xy, normal.xy));

	v.vertex.xyz += _Displacement * height * v.normal;
}

void WaterSurf_half(Input IN, inout SurfaceOutputStandard o)
{
	float2 uv0 = _Time.y * _FlowSpeed + TRANSFORM_TEX(IN.tex, _NormalTex);
	float2 uv1 = float2(uv0.y, -uv0.x);
	half4 packedNormal = UNITY_SAMPLE_TEX2D(_NormalTex, uv0);
	packedNormal += UNITY_SAMPLE_TEX2D(_NormalTex, uv1);
	half3 normal = UnpackNormalWithScale(packedNormal / 2.0, _Distortion);
	normal.z = sqrt(1.0 - dot(normal.xy, normal.xy));

	half3x3 tangentToView = half3x3(IN.viewTangent, IN.viewBitangent, IN.viewNormal);
	half3 viewNormal = normalize(mul(normal, tangentToView));
	
	half4 color = 1;
#ifdef REFRACTION_ON
	const half g = (sqrt(5.0) - 1.0) / 2.0;
	const half count = _Samples;
	half3 clipNormal = mul((float3x3)UNITY_MATRIX_P, viewNormal - IN.viewNormal);
	half2 uv = IN.grabPos.xy + _ScreenDistortion * clipNormal.xy * saturate(rcp(abs(IN.viewPos.z)));
	color.rgb = 0;
	for(half x = count - 1.0; x >= 0.0; x -= 1.0)
	{
		half angle = radians(x * g * 360.0);
		half2 len = sqrt(x) * _Blur * g * _CameraOpaqueTexture_TexelSize.xy;
		color.rgb += SampleSceneColor(uv + len * half2(cos(angle), sin(angle)));
	}
	color.rgb /= count;
	//color.rgb = 10;
#endif

	
#ifdef ALPHABLEND_ON
	half depth = length(IN.viewPos);
	half3 sceneViewPos = IN.viewPos;
#else
	half3 viewDirection = normalize(IN.viewPos);
	half zFactor = rcp(abs(viewDirection.z));
	half sceneDepth = LinearEyeDepth(SampleSceneDepth(IN.grabPos.xy));
	half depth = (sceneDepth - abs(IN.viewPos.z)) * zFactor;
	bool isUnderwater = depth > 0.0;
	depth = isUnderwater ? depth : _ProjectionParams.z;
	half3 sceneViewPos = sceneDepth * zFactor * viewDirection;
#endif
	half d = saturate(exp2(-_WaterDepth * (depth * depth)));
	half4 tint = lerp(_DeepColor, _ShallowColor, d);
	color *= tint;

	if(UseFoam)
	{
		half foam = 1.0 - normal.z;

		half width = 0.4;
		half sharpness = 2.0;
		half sharewaweDistortion = 0.3;
		float2 foamUV = TRANSFORM_TEX(IN.tex, _FoamTex);
		foamUV = foamUV + sharewaweDistortion * normal.xy;
		half wave = UNITY_SAMPLE_TEX2D(_FoamTex, foamUV).r;
		half layer = _ShorelineWidth * dot(IN.viewNormal, IN.viewPos - sceneViewPos);
		half shoreline = saturate(1 - layer);
		half t = shoreline - _Time.y * _Shoreline_Wave_Speed;
		t = saturate((abs(frac(t) - 0.5) - 0.5 + 0.5 * width) * sharpness + width);
		foam = saturate(t * shoreline - wave + foam * _Foam + _FoamOffset);
		
		color = lerp(color, 1.0, foam);
		o.Smoothness = exp2(-foam * 5.0);
	}
	else
	{
		o.Smoothness = 1.0;
	}

	o.Albedo = color.rgb;
	o.Alpha = color.a;
	o.Normal = normal;
}

void vert (inout appdata_full v, out Input o) 
{
	UNITY_INITIALIZE_OUTPUT(Input, o);

	WaterVert_half(v);

	float3 worldNormal = mul((float3x3)UNITY_MATRIX_M, v.normal);
	float3 worldTangent = mul((float3x3)UNITY_MATRIX_M, v.tangent.xyz);
	float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

	float3 worldPos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0));
	o.viewPos = mul(UNITY_MATRIX_V, float4(worldPos, 1.0));
	o.viewNormal = mul((float3x3)UNITY_MATRIX_V, worldNormal);
	o.viewTangent = mul((float3x3)UNITY_MATRIX_V, worldTangent);
	o.viewBitangent = mul((float3x3)UNITY_MATRIX_V, worldBinormal);

	float4 clipPos = mul(UNITY_MATRIX_P, float4(o.viewPos, 1.0));
	o.grabPos = ComputeGrabScreenPos(clipPos);
	o.tex = v.texcoord;
}

void surf (Input IN, inout SurfaceOutputStandard o)
{
	IN.grabPos = IN.grabPos / IN.grabPos.w;
	IN.viewNormal = normalize(IN.viewNormal);
	IN.viewTangent  = normalize(IN.viewTangent);
	IN.viewBitangent = normalize(IN.viewBitangent);
	WaterSurf_half(IN, o);
}