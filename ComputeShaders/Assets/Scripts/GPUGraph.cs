using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a Unity component script used to manage the graph game object general behavior.
/// </summary>
public class GPUGraph : MonoBehaviour
{
    #region Fields / Properties

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private ComputeShader computeShader;

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private Material material;

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private Mesh mesh;

    /// <summary>
    /// Instance variable <c>resolution</c> represents the resolution value of the graph.
    /// </summary>
    [SerializeField, Range(10, 1000)]
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
    /// TODO: add comment
    /// </summary>
    private FunctionLibrary.FunctionName transitionFunction;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private ComputeBuffer positionsBuffer;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private static readonly int positionsId = Shader.PropertyToID("_Positions");

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private static readonly int stepId = Shader.PropertyToID("_Step");

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private static readonly int timeId = Shader.PropertyToID("_Time");

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // To avoid objects getting destroyed on hot reload.
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
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
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
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