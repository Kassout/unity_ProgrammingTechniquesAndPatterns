using UnityEngine;
using static UnityEngine.Mathf;

/// <summary>
/// Class <c>FunctionLibrary</c> is a script used as a library to host some general utils functions.
/// </summary>
public static class FunctionLibrary
{
    #region Fields / Properties

    /// <summary>
    /// Instance variable <c>Function</c> represents a reference to a graph wave shape computer function.
    /// </summary>
    public delegate Vector3 Function(float u, float v, float t);

    /// <summary>
    /// Instance variable <c>FunctionName</c> represents an enumeration of the different graph wave shape computer function's name available.
    /// </summary>
    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

    /// <summary>
    /// Instance variable <c>functions</c> is an Array of <c>Function</c> representing the different graph wave shape computer functions.
    /// </summary>
    private static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    #endregion

    #region Public

    /// <summary>
    /// This function is used to get the function associated to the index of the given entry value.
    /// </summary>
    /// <param name="name">A <c>FunctionName</c> enumeration value representing the name of the function to get.</param>
    /// <returns>A <c>Function</c> delegate representing the graph wave shape computer function chosen.</returns>
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    /// <summary>
    /// This function is used to compute the y position of a point applying a wave effect shapes.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    /// <summary>
    /// This function is used to compute the y position of a point applying multiple wave effect shapes.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>
    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    /// <summary>
    /// This function is used to compute the y position of a point applying a ripple effect shape.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>
    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }

    /// <summary>
    /// This function is used to compute the point positions to apply a sphere shape to the graph.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>
    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    /// <summary>
    /// This function is used to compute the point positions to apply a torus shape to the graph.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>
    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    #endregion
}