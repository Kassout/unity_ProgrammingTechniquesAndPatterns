using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a Unity component script used to manage the graph game object general behavior.
/// </summary>
public class Graph : MonoBehaviour
{
    /// <summary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private Transform pointPrefab;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    [SerializeField, Range(10, 100)]
    private int resolution = 10;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        float step = 2f / resolution;
        Vector3 position =  Vector3.zero;
        Vector3 scale = Vector3.one * step;
        for (int i = 0; i < resolution; i++)
        {
            Transform point = Instantiate(pointPrefab);
            position.x = (i + 0.5f) * step - 1f;
            position.y = position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }
}
