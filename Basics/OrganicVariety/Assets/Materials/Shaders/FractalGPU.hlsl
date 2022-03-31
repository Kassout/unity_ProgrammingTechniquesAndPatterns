// Get graph point positions from GPU buffer.
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3x4> _Matrices;
#endif

// Function to instance our points proceduraly.
void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		// Retrieve the transformation matrix of the fractal part by indexing the transformation matrix buffer with the identifier of the instance that's currently being drawn.
		float3x4 m = _Matrices[unity_InstanceID];
		unity_ObjectToWorld._m00_m01_m02_m03 = m._m00_m01_m02_m03;
		unity_ObjectToWorld._m10_m11_m12_m13 = m._m10_m11_m12_m13;
		unity_ObjectToWorld._m20_m21_m22_m23 = m._m20_m21_m22_m23;
		unity_ObjectToWorld._m30_m31_m32_m33 = float4(0.0, 0.0, 0.0, 1.0);
	#endif
}

float4 _ColorA, _ColorB;
// Offset values to support each level having a different starting color.
float4 _SequenceNumbers;

float4 GetFractalColor() {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float4 color;
		// Compute a sequence to color fractal parts so it looks organic.
		color.rgb = lerp(_ColorA.rgb, _ColorB.rgb, frac(unity_InstanceID * _SequenceNumbers.x + _SequenceNumbers.y));
		// Use color transparency variable as a smoothness variable because we don't use transparency in our program.
		color.a = lerp(_ColorA.a, _ColorB.a, frac(unity_InstanceID * _SequenceNumbers.z + _SequenceNumbers.w));
		return color;
	#else
		return _ColorA;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float4 FractalColor) {
	Out = In;
	FractalColor = GetFractalColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half4 FractalColor) {
	Out = In;
	FractalColor = GetFractalColor();
}