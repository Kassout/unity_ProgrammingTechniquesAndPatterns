using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Use Unity Mathematics library for Burst compilation (incompatible with classic Quaternion not design with vectorization in mind).
using static Unity.Mathematics.math;
// Indicate how to interpret the types of quaternion because of methods name conflicts with Mathf library.
using quaternion = Unity.Mathematics.quaternion;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>Fractal</c> is a Unity script used to manage the fractal generation behavior.
/// </summary>
public class Fractal : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Structure <c>FractalPart</c> define the different element describing a fractal part.
    /// </summary>
    private struct FractalPart
    {
        public float3 worldPosition;
        public quaternion rotation, worldRotation;
        public float maxSagAngle, spinAngle, spinVelocity;
    }

    /// <summary>
    /// Structure <c>UpdateFractalLevelJob</c> define the fractal updating level behavior using the Unity.Jobs <c>JobFor</c> interface for improved performance.
    /// </summary>
    // FloatPrecision controls the precision of the sin and cos methods.
    // FloatMode on "Fast" allows Burst to reorder mathematical operations (because of floating-point limitations changer the order will produce slightly different results).
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    private struct UpdateFractalLevelJob : IJobFor
    {
        public float scale;
        public float deltaTime;

        [ReadOnly]
        public NativeArray<FractalPart> parents;
        public NativeArray<FractalPart> parts;

        [WriteOnly]
        public NativeArray<float3x4> matrices;

        public void Execute(int i)
        {
            FractalPart parent = parents[i / 5];
            FractalPart part = parts[i];
            part.spinAngle += part.spinVelocity * deltaTime;

            // Find the sagging rotation axis.
            float3 upAxis = mul(mul(parent.worldRotation, part.rotation), up());
            float3 sagAxis = cross(up(), upAxis);

            // Don't apply sagging to a straight up part => system in equilibrium.
            float sagMagnitude = length(sagAxis);
            quaternion baseRotation;
            if (sagMagnitude > 0f) {
                sagAxis /= sagMagnitude;
                quaternion sagRotation = quaternion.AxisAngle(sagAxis, part.maxSagAngle * sagMagnitude);
                baseRotation = mul(sagRotation, parent.worldRotation);
            } else {
                baseRotation = parent.worldRotation;
            }

            // mul == Multiplication() from Mathematics library.
            part.worldRotation = mul(baseRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));
            part.worldPosition = parent.worldPosition + mul(part.worldRotation, float3(0f, 1.5f * scale, 0f));
            parts[i] = part;

            // To create an uniform vector with the Mathematics library, use float3(value).
            float3x3 r = float3x3(part.worldRotation) * scale;
            matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
        }
    }

    /// <summary>
    /// Instance variable <c>parts</c> is a list of Unity <c>NativeArray</c> collections representing the different parts of the fractal.
    /// </summary>
    private NativeArray<FractalPart>[] parts;

    /// <summary>
    /// Instance variable <c>matrices</c> is a list of Unity <c>float3x4</c> collections representing the transformation matrices for the rendering of the fractal parts.
    /// </summary>
    private NativeArray<float3x4>[] matrices;

    /// <summary>
    /// Instance variable <c>depth</c> represents the depth value of fractal generation.
    /// </summary>
    /// Depth start at least at 2 to avoid some division by 0 errors which can happened with a depth of 0.
    [SerializeField, Range(3, 8)]
    private int depth = 4;

    /// <summary>
    /// Instance variable <c>mesh</c> is a Unity <c>Mesh</c> component representing the mesh of the model of the fractal parts.
    /// </summary>
    [SerializeField]
    private Mesh mesh;

        /// <summary>
    /// Instance variable <c>leafMesh</c> is a Unity <c>Mesh</c> component representing the mesh of the model of the fractal leaf parts.
    /// </summary>
    [SerializeField]
    private Mesh leafMesh;

    /// <summary>
    /// Instance variable <c>material</c> is a Unity <c>Material</c> object representing the material of the model of the fractal parts.
    /// </summary>
    [SerializeField]
    private Material material;

    /// <summary>
    /// Instance variable <c>gradientA</c> is a Unity <c>Gradient</c> object representing the first gradient color sent to the fractal shader to paint the different fractal level.
    /// </summary>
    [SerializeField]
    private Gradient gradientA;

    /// <summary>
    /// Instance variable <c>gradientB</c> is a Unity <c>Gradient</c> object representing the second gradient color sent to the fractal shader to paint the different fractal level.
    /// </summary>
    [SerializeField]
    private Gradient gradientB;

    /// <summary>
    /// Instance variable <c>leafColorA</c> is a Unity <c>Color</c> structure representing the first leaf color sent to the fractal shader to paint the leaf instances of the fractal.
    /// </summary>
    [SerializeField]
    private Color leafColorA;

    /// <summary>
    /// Instance variable <c>leafColorB</c> is a Unity <c>Color</c> structure representing the second leaf color sent to the fractal shader to paint the leaf instances of the fractal.
    /// </summary>
    [SerializeField]
    private Color leafColorB;

    /// <summary>
    /// Instance variable <c>maxSagAngleA</c> represent the first maximum sagging angle value allowed for a fractal part.
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    private float maxSagAngleA = 15f;

    /// <summary>
    /// Instance variable <c>maxSagAngleB</c> represent the second maximum sagging angle value allowed for a fractal part.
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    private float maxSagAngleB = 25f;

    /// <summary>
    /// Instance variable <c>spinVelocityA</c> represent the first maximum spin velocity value allowed for a fractal part.
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    private float spinVelocityA = 20f;
    
    /// <summary>
    /// Instance variable <c>spinVelocityB</c> represent the second maximum spin velocity value allowed for a fractal part.
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    private float spinVelocityB = 25f;

    /// <summary>
    /// Instance variable <c>reverseSpinChance</c> represent the chance value for a fractal part spin to reverse.
    /// </summary>  
    [SerializeField, Range(0f, 1f)]
    private float reverseSpinChance = 0.25f;

    /// <summary>
    /// Instance variable <c>rotations</c> is a list of Unity <c>quaternion</c> structures representing the different fractal parts generation rotations.
    /// </summary>
    private static quaternion[] rotations = {
        quaternion.identity,
        // Replace Quaternion.Euler() with math library functions equivalent. Mathematics library works with radians instead of degrees.
        quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI),
        quaternion.RotateX(0.5f * PI), quaternion.RotateX(-0.5f * PI)
    };

    /// <summary>
    /// Instance variable <c>matricesBuffers</c> is a list of Unity <c>ComputeBuffer</c> objects representing a memory program space to store the different transformation matrices of the fractal parts on the GPU.
    /// </summary>
    private ComputeBuffer[] matricesBuffers;

    /// <summary>
    /// Instance variable <c>matricesId</c> represents the identifier value of a <c>_Matrices</c> shader property.
    /// </summary>
    private static readonly int matricesId = Shader.PropertyToID("_Matrices");

    /// <summary>
    /// Instance variable <c>colorAId</c> represents the identifier value of a <c>_ColorA</c> shader property.
    /// </summary>
    private static readonly int colorAId = Shader.PropertyToID("_ColorA");

    /// <summary>
    /// Instance variable <c>colorBId</c> represents the identifier value of a <c>_ColorB</c> shader property.
    /// </summary>
    private static readonly int colorBId = Shader.PropertyToID("_ColorB");

    /// <summary>
    /// Instance variable <c>sequenceNumbersId</c> represents the identifier value of a <c>_SequenceNumbers</c> shader property.
    /// </summary>
    private static readonly int sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");

    /// <summary>
    /// Instance variable <c>propertyBlock</c> is a Unity <c>MaterialPropertyBlock</c> object representing the material draw command manager of the fractal compute buffer.
    /// </summary>
    private static MaterialPropertyBlock propertyBlock;

    /// <summary>
    /// Instance variable <c>sequenceNumbers</c> is a Unity <c>Vector4</c> structure representing TODO: add comment.
    /// </summary>
    private Vector4[] sequenceNumbers;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Initialize fractal parts containers size.
        // For the jobs system of Unity we need to use native array as struct that contains pointer to native machine memory, 
        // existing outside the regular managed memory heap used by C# code.
        parts = new NativeArray<FractalPart>[depth];
        matrices = new NativeArray<float3x4>[depth];

        // Initialize compute buffers.
        matricesBuffers = new ComputeBuffer[depth];

        sequenceNumbers = new Vector4[depth];

        // The stride represents the number of bytes needed to store and manipulate the object type of the parameter passed to the compute buffers.
        // Here a 3x4 matrix has 12 float values, so the stride of the buffers is 12 x 4 bytes.
        int stride = 12 * 4;

        // Length per depth = 5 because we give 5 childs to every parent of the fractal (Up, left, right, forward and back).
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
        {
            // Allocator of the native array to indicate for how long it is expected to exist.
            parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
            matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
            matricesBuffers[i] = new ComputeBuffer(length, stride);
            // Setup an arbitrary and different sequence numbers per level. => For fractal parts painting color and smoothness choices.
            sequenceNumbers[i] = new Vector4(Random.value, Random.value, Random.value, Random.value);
        }

        // Create first element (root).
        parts[0][0] = CreatePart(0);
        // Create childs.
        for (int li = 1; li < parts.Length; li++)
        {
            NativeArray<FractalPart> levelParts = parts[li];

            // For each parent, create 5 childs (Up, left, right, forward and back).
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(ci);
                }
            }
        }

        // Assign only when current value is null.
        propertyBlock ??= new MaterialPropertyBlock();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();
            parts[i].Dispose();
            matrices[i].Dispose();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
        sequenceNumbers = null;
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // To animate the fractal
        float deltaTime = Time.deltaTime;

        // Initialize root fractal parts
        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += rootPart.spinAngle * deltaTime;
        rootPart.worldRotation = mul(transform.rotation, mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle)));
        rootPart.worldPosition = transform.position;
        parts[0][0] = rootPart;

        float objectScale = transform.lossyScale.x;

        // We store the transform component data inside a static 3x4 matrix.
        // TRS method stands for translation-rotation-scale.
        // No TRS method for float3x4 matrices, we have to assemble the matrix ourselves.
        float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
        matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);

        float scale = objectScale;

        JobHandle jobHandle = default;
        // Using generated fractal parts data computed and stored on "Awake", now initialize all the fratcal parts to position and rotate them accordingly.
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;

            // Initialize UpdateFractalLevel job.
            jobHandle = new UpdateFractalLevelJob
            {
                deltaTime = deltaTime,
                scale = scale,
                parents = parts[li - 1],
                parts = parts[li],
                matrices = matrices[li]
                // Update fractal level processes migrated to a Unity job.
                // Use Schedule to let the job perform the function look on its on, exploiting the best of parallel computing.
                // The second argument of the function is used to enforce a sequential dependency between jobs. Here we have none.
                // 5 is the batch count representing the number of jobs a thread is allowed to iterate over in a single loop.
            }.ScheduleParallel(parts[li].Length, 5, jobHandle);
        }
        // Execute job after scheduling them all.
        jobHandle.Complete();

        // Upload the matrices to the GPU on all buffers.
        // Setup bounds to define the drawing area of the fractal available to the GPU.
        // If you compute the maximum diameter of the fractal, with a diameter of 1 for the root element, you tend to 3 on infinite depth level. 
        Bounds bounds = new Bounds(rootPart.worldPosition, 3f * float3(objectScale));

        // the fractal leaves are supposed to be the last level elements.
        int leafIndex = matricesBuffers.Length - 1;
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = matricesBuffers[i];
            // Communicate parameter values to the compute buffer.
            buffer.SetData(matrices[i]);

            // Dissociate fractal leaf level colors/mesh models from the other fractal level
            Color colorA, colorB;
            Mesh instanceMesh;
            if (i == leafIndex) {
                colorA = leafColorA;
                colorB = leafColorB;
                instanceMesh = leafMesh;
            } else {
                // Change colors depending depth level of the current fractal generation loop.
                // Use 2 gradients to mix colors and level
                float gradientInterpolator = i / (matricesBuffers.Length - 1f);
                colorA = gradientA.Evaluate(gradientInterpolator);
                colorB = gradientB.Evaluate(gradientInterpolator);
                instanceMesh = mesh;
            }
            propertyBlock.SetColor(colorAId, colorA);
            propertyBlock.SetColor(colorBId, colorB);
            
            // Link the transformation matrix buffer to the matrices property of the material property block.
            // Using material property block instead of direct material will make Unity copy the configuration that the block has at that time and use it for specific draw command
            // Overrulling what was set for the material.
            propertyBlock.SetBuffer(matricesId, buffer);
            propertyBlock.SetVector(sequenceNumbersId, sequenceNumbers[i]);
            // Draw mesh using the different properties of given material and bounds.
            Graphics.DrawMeshInstancedProcedural(instanceMesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is reponsible for creating a new fractal part.
    /// </summary>
    private FractalPart CreatePart(int childIndex) => new FractalPart
    {
        maxSagAngle = radians(Random.Range(maxSagAngleA, maxSagAngleB)),
        rotation = rotations[childIndex],
        spinVelocity = (Random.value < reverseSpinChance ? -1f : 1f) * radians(Random.Range(spinVelocityA, spinVelocityB))
    };

    #endregion
}