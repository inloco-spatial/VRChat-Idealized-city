#if defined(UNITY_SINGLE_PASS_STEREO)
	float2 StereoTransformScreenSpaceTex(float2 uv)
	{
		// TODO: RVS support can be added here, if Universal decides to support it
		float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
		return saturate(uv) * scaleOffset.xy + scaleOffset.zw;
	}
#else
	#define StereoTransformScreenSpaceTex(uv) uv
#endif
/*
#ifndef REQUIRE_DEPTH_TEXTURE
Texture2D _CameraDepthTexture;
SamplerState sampler_CameraDepthTexture;
#endif
*/
#ifndef REQUIRE_OPAQUE_TEXTURE
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
Texture2DArray _CameraOpaqueTexture;
#else
Texture2D _CameraOpaqueTexture;
#endif
SamplerState sampler_CameraOpaqueTexture;
#endif

half3 SampleSceneColor(half2 uv)
{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
	return _CameraOpaqueTexture.Sample(sampler_CameraOpaqueTexture, float3(StereoTransformScreenSpaceTex(uv), unity_StereoEyeIndex)).rgb;
#else
	return _CameraOpaqueTexture.Sample(sampler_CameraOpaqueTexture, StereoTransformScreenSpaceTex(uv)).rgb;
#endif
}

half4 _CameraOpaqueTexture_TexelSize;

void WaterVert_half(half3 ObjectPosition, half3 ObjectNormal, half3 ObjectTangent, half3 ObjectBitangent, half2 UV0, half2 UV1, out half3 VertexPosition, out half3 VertexNormal, out half3 VertexTangent)
{
	half4 packedNormal = _NormalTex.SampleLevel(sampler_NormalTex, UV0, _DisplacementLOD);
	packedNormal += _NormalTex.SampleLevel(sampler_NormalTex, UV1, _DisplacementLOD);
	half3 normal = UnpackNormal(packedNormal / 2.0);
	half height = 1.0 - sqrt(1.0 - dot(normal.xy, normal.xy));

	VertexPosition = ObjectPosition + _Displacement * height * ObjectNormal;
	VertexNormal = ObjectNormal;
	VertexTangent = ObjectTangent;
}

void WaterSurf_half(half3 ViewVector, half3 ViewNormal, half3 ViewTangent, half3 ViewBitangent, half4 ScreenPosition, half2 uv0, half2 uv1, half2 foamUV, half3 GrabTexel, half Depth, out half3 Albedo, out half Smoothness, out half3 Emission, out half3 TangentNormal, out half3 TangentWorld)
{
	half4 packedNormal = _NormalTex.Sample(sampler_NormalTex, uv0);
	packedNormal += _NormalTex.Sample(sampler_NormalTex, uv1);
	half3 normal = UnpackNormalScale(packedNormal / 2.0, _Distortion);
	normal.z = sqrt(1.0 - dot(normal.xy, normal.xy));
	TangentNormal = normal;

	half3x3 tangentToView = half3x3(ViewTangent, ViewBitangent, ViewNormal);
	half3 viewNormal = normalize(mul(normal, tangentToView));
	TangentWorld = mul((float3x3)UNITY_MATRIX_I_V, viewNormal);
	viewNormal -= ViewNormal;
	half3 clipNormal = mul((float3x3)GetViewToHClipMatrix(), viewNormal);
	
	const half g = (sqrt(5.0) - 1.0) / 2.0;
	const half count = 8;
	half2 uv = ScreenPosition.xy + _ScreenDistortion * clipNormal.xy * saturate(rcp(ViewVector.z));
	half3 color = 0;
	for(half x = count; x > 0.0; x -= 1.0)
	{
		half angle = radians(x * g * 360.0);
		half2 len = sqrt(x) * g * _CameraOpaqueTexture_TexelSize.xy;
		color += SampleSceneColor(uv + len * half2(cos(angle), sin(angle)));
	}
	color /= count;

	half3 viewDirection = normalize(ViewVector);
	//half sceneDepth = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
	half depth = (Depth - ViewVector.z) / viewDirection.z;
	bool isUnderwater = depth > 0.0;
	depth = isUnderwater ? depth : _ProjectionParams.z;
	half3 viewPos = viewDirection / viewDirection.z * Depth;
	half d = saturate(exp2(-_WaterDepth * (depth * depth)));
	half3 tint = lerp(_DeepColor, _ShallowColor, d).rgb;
	color *= tint;

	if(UseFoam)
	{
		half foam = 1.0 - normal.z;

		half width = 0.4;
		half sharpness = 2.0;
		half sharewaweDistortion = 0.3;
		foamUV = foamUV + sharewaweDistortion * TangentNormal.xy;
		half wave = _FoamTex.Sample(sampler_FoamTex, foamUV).r;
		half layer = _ShorelineWidth * dot(ViewNormal, viewPos - ViewVector);
		half shoreline = saturate(1 - layer);
		half t = shoreline - _Time.y * _Shoreline_Wave_Speed;
		t = saturate((abs(frac(t) - 0.5) - 0.5 + 0.5 * width) * sharpness + width);
		foam = saturate(t * shoreline - wave + foam * _Foam + _FoamOffset);
		
		Albedo = lerp(color, 1.0, foam);
		Smoothness = exp2(-foam * 5.0);
	}
	else
	{
		Albedo = color;
		Smoothness = 1.0;
	}
	Emission = 0;
}