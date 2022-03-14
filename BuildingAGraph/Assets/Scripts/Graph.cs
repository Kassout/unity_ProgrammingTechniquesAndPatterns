using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a Unity component script used to manage the graph game object general behavior.
/// </summary>
public class Graph : MonoBehaviour
{
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

    /// <summary>
    /// Instance variable <c>points</c> is an array of Unity <c>Transform</c> structures representing the position, rotation and scale of the displayed graph points.
    /// </summary>
    private Transform[] points;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        float step = 2f / resolution;
        Vector3 position =  Vector3.zero;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);

            position.x = (i + 0.5f) * step - 1f;

            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        float time = Time.time;
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i];

            Vector3 position = point.localPosition;
            position.y = ComputeSineWave(position.x, time);
            point.localPosition = position;
        }
    }

    /// <summary>
    /// This function is used to compute the y position of the points applying the sinus wave form.
    /// </summary>
    /// <param name="xPosition">A float value representing the x position of the given point.</param>
    /// <param name="time">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A float value representing the computed y position of the given point.</returns>
    private float ComputeSineWave(float xPosition, float time) {
        return Mathf.Sin(Mathf.PI * (xPosition + time));
    }
}