// Shader definition
Shader "Graph/Point Surface" {

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
        #pragma surface ConfigureSurface Standard fullforwardshadows

        // Sets a minimum for the shader's target level and quality.
        #pragma target 3.0

        // Define input structure for our configuration function. 
        struct Input {
            // Vector3 struct containing the world position of what gets rendered.
            float3 worldPos;
        };

        float _Smoothness;

        // use inout to indicates that the object both passed o the function and used for the result of the function.
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            surface.Albedo.rg = saturate(input.worldPos.xy * 0.5 + 0.5);
            surface.Smoothness = _Smoothness;
        }

        ENDCG
    }

    // Standard shader fallback
    FallBack "Diffuse"
}