using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a Unity component script used to manage the graph game object general behavior.
/// </summary>
public class GPUGraph : MonoBehaviour
{
    #region Fields / Properties

    const int maxResolution = 1000;

    /// <summmary>
    /// Instance variable <c>computeShader</c> is a Unity <c>ComputeShader</c> object representing the shader program to execute GPU computations.
    /// </summary>
    [SerializeField]
    private ComputeShader computeShader;

    /// <summmary>
    /// Instance variable <c>material</c> is a Unity <c>Material</c> object representing the rendered material of our graph points.
    /// </summary>
    [SerializeField]
    private Material material;

    /// <summmary>
    /// Instance variable <c>mesh</c> is a Unity <c>Mesh</c> object representing the mesh structure of our graph points.
    /// </summary>
    [SerializeField]
    private Mesh mesh;

    /// <summary>
    /// Instance variable <c>resolution</c> represents the resolution value of the graph.
    /// </summary>
    [SerializeField, Range(10, maxResolution)]
    private int resolution = 10;

    /// <summmary>
    /// Instance variable <c>function</c> represents the selection name associated with a wave function to use for the graph.
    /// </summary>
    [SerializeField]
    private FunctionLibrary.FunctionName function = default;

    /// <summary>
    /// Enumeration variable <c>TransitionMode</c> representing the different graph transition mode available.
    /// </summary>
    public enum TransitionMode { Cycle, Random };

    /// <summmary>
    /// Instance variable <c>transitionMode</c> represents the graph transition mode name currently selected.
    /// </summary>
    [SerializeField]
    private TransitionMode transitionMode;

    /// <summmary>
    /// Instance variable <c>functionDuration</c> represents the duration value of the function to display.
    /// </summary>
    [SerializeField, Min(0f)]
    private float functionDuration = 1f;

    /// <summmary>
    /// Instance variable <c>transitionDuration</c> represents the duration value of the transition between two functions to display.
    /// </summary>
    [SerializeField, Min(0f)]
    private float transitionDuration = 1f;

    /// <summary>
    /// Instance variable <c>duration</c> represents the current displayed duration value of the graph function.
    /// </summary>
    private float duration;

    /// <summary>
    /// Instance variable <c>transitioning</c> represents the current transitioning status of the graph.
    /// </summary>
    private bool transitioning = true;

    /// <summary>
    /// Instance variable <c>transitionFunction</c> is a <c>FunctionName</c> enumeration value representing the current function to transition from.
    /// </summary>
    private FunctionLibrary.FunctionName transitionFunction;

    /// <summary>
    /// Instance variable <c>positionsBuffer</c> is a Unity <c>ComputeBuffer</c> object representing a memory program space to store the graph point positions on the GPU.
    /// </summary>
    private ComputeBuffer positionsBuffer;

    /// <summary>
    /// Instance variable <c>positionsId</c> represents the identifier value of a <c>_Positions</c> shader property.
    /// </summary>
    private static readonly int positionsId = Shader.PropertyToID("_Positions");

    /// <summary>
    /// Instance variable <c>resolutionId</c> represents the identifier value of a <c>_Resolution</c> shader property.
    /// </summary>
    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");

    /// <summary>
    /// Instance variable <c>stepId</c> represents the identifier value of a <c>_Step</c> shader property.
    /// </summary>
    private static readonly int stepId = Shader.PropertyToID("_Step");

    /// <summary>
    /// Instance variable <c>timeId</c> represents the identifier value of a <c>_Time</c> shader property.
    /// </summary>
    private static readonly int timeId = Shader.PropertyToID("_Time");

    /// <summary>
    /// Instance variable <c>transitionProgressId</c> represents the identifier value of a <c>_TransitionProgess</c> shader property.
    /// </summary>
    private static readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgess");

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // To avoid objects getting destroyed on hot reload.
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    #endregion

    #region Private

    private void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;

        // Communicate parameter values to the compute shader program.
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, duration / transitionDuration));
        }

        // Find the kernel associated with the function/transition function we are looking for.
        // Kernel are functions of the shader program, you can call them by using ids associate to them.
        int kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * FunctionLibrary.FunctionCount;
        // Link the positions buffer to the positions property of one kernel of the compute shader.
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
        // Groups in a number of thread work partitions for the GPU to instructed to execute a compute shader function.
        // GPU schedules groups to run independently and in parallel.
        // Here we have 8 threads per group so to perform our position computation we will need resolution value / 8 threads, groups.
        int groups = Mathf.CeilToInt(resolution / 8f);
        // Invoke dispatch to run the chosen kernel. Other parameters are the number of groups for each dimension (x, y, z);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        // Link the positions buffer to the positions property of the material.
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));

        // Draw mesh using the different properties of given material and bounds.
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    /// <summary>
    /// This function is used to select the next function to display on the graph.
    /// </summary>
    private void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ? FunctionLibrary.GetNextFunctionName(function) : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    #endregion
}