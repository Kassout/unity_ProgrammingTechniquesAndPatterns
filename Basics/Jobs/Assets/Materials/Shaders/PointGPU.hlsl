// Get graph point positions from GPU buffer.
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3> _Positions;
#endif

// Step property.
float _Step;

// Function to instance our points proceduraly.
void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		// Retrieve the position of the point by indexing the positions buffer with the identifier of the instance that's currently being drawn.
		float3 position = _Positions[unity_InstanceID];

		// unity_ObjectToWorld is a transformation matrix for the points.
		// Set the matrix values to 0.
		unity_ObjectToWorld = 0.0;
		// Set the last matrix column (= point position) to the position vector + 1.0 value in the last row.
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
		// Assert the matrix diagonal (= scale of the point) values to the _Step.
		unity_ObjectToWorld._m00_m11_m22 = _Step;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}