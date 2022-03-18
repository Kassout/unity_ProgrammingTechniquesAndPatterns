// Shader definition
Shader "Graph/Point Surface GPU" {

   // Add configuration option appear in the Unity editor.
    Properties {
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
    }

    // SubShader definition
    SubShader {
        // Code section
        CGPROGRAM

        // Instruct the shader compiler to generate a "surface" shader with "Standard" lighting and full support for shadows "fullforwardshadows".
        // "ConfigureSurface" refers to a method used to configure the shader.
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma editor_sync_compilation

        // Sets a minimum for the shader's target level and quality.
        #pragma target 4.5

        // Define input structure for our configuration function. 
        struct Input {
            // Vector3 struct containing the world position of what gets rendered.
            float3 worldPos;
        };

        float _Smoothness;

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float3> _Positions;
        #endif

        float _Step;

        void ConfigureProcedural () {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Positions[unity_InstanceID];

                unity_ObjectToWorld = 0.0;
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
                unity_ObjectToWorld._m00_m11_m22 = _Step;
            #endif
        }

        // use inout to indicates that the object both passed o the function and used for the result of the function.
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
            surface.Smoothness = _Smoothness;
        }

        ENDCG
    }

    // Standard shader fallback
    FallBack "Diffuse"
}