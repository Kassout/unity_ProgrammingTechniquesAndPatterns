// Shader definition
Shader "Fractal/Fractal Surface GPU" {

    // SubShader definition
    SubShader {
        // Code section
        CGPROGRAM

        // Instruct the shader compiler to generate a "surface" shader with "Standard" lighting and full support for shadows "fullforwardshadows".
        // "ConfigureSurface" refers to a method used to configure the shader.
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        // Add options on shader instancing :
        // Invoke "ConfigureProcedural" function per vertex.
        // "assumeuniformscaling" to ignore nonuniform deformation on shader.
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        // Force Unity to stall and immediately compile the shader right before it gets used the first time, avoiding the dummy shader.
        #pragma editor_sync_compilation

        // Sets a minimum for the shader's target level and quality.
        #pragma target 4.5

        // Include "PointGPU" for "ConfigureProcedural" function.
        #include "FractalGPU.hlsl"

        // Define input structure for our configuration function. 
        struct Input {
            // Vector3 struct containing the world position of what gets rendered.
            float3 worldPos;
        };

        float _Smoothness;

        // use inout to indicates that the object both passed o the function and used for the result of the function.
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            // Albedo represents the shader color.
            surface.Albedo = GetFractalColor().rgb;
            surface.Smoothness = GetFractalColor().a;
        }

        ENDCG
    }

    // Standard shader fallback
    FallBack "Diffuse"
}