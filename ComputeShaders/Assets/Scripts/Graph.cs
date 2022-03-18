using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a Unity component script used to manage the graph game object general behavior.
/// </summary>
public class Graph : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance variable <c>pointPrefab</c> is a Unity <c>Transform</c> structure representing position, rotation and scale of the point pre-fabricated object.
    /// </summary>
    [SerializeField]
    private Transform pointPrefab;

    /// <summary>
    /// Instance variable <c>resolution</c> represents the resolution value of the graph.
    /// </summary>
    [SerializeField, Range(10, 200)]
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
    /// Instance variable <c>points</c> is an array of Unity <c>Transform</c> structures representing the position, rotation and scale of the displayed graph points.
    /// </summary>
    private Transform[] points;

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

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Compute distance separation between points.
        float step = 2f / resolution;

        // Scale cube(=points) dimensions depending graph step.
        Vector3 scale = Vector3.one * step;

        // Compute number of points depending graph resolution.
        points = new Transform[resolution * resolution];

        // Instanciate cubes(=points)
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);

            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning) {
            if (duration >= transitionDuration) {
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

        if (transitioning) {
            UpdateFunctionTransition();
        } else {
            UpdateFunction();
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is used to update the current displayed function on the graph.
    /// </summary>
    private void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }

    /// <summary>
    /// This function is used to update the current displayed function on the graph applying a transition.
    /// </summary>
    private void UpdateFunctionTransition()
    {
        FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitionFunction);
        FunctionLibrary.Function to = FunctionLibrary.GetFunction(function);
        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
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