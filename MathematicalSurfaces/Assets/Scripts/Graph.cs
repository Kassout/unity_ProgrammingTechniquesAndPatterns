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
    [SerializeField, Range(10, 100)]
    private int resolution = 10;

    /// <summmary>
    /// Instance variable <c>function</c> represents the selection name associated with a wave function to use for the graph.
    /// </summary>
    [SerializeField]
    private FunctionLibrary.FunctionName function;

    /// <summary>
    /// Instance variable <c>points</c> is an array of Unity <c>Transform</c> structures representing the position, rotation and scale of the displayed graph points.
    /// </summary>
    private Transform[] points;

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

    #endregion
}