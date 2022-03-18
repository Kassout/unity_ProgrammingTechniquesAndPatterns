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
    /// Enumeration variable <c>FunctionName</c> representing the different graph wave shape computer function's name available.
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
    public static Function GetFunction(FunctionName name) => functions[(int)name];

    /// <summary>
    /// This function is used to get the next function following the current one in the list of defined function in the <c>FunctionName</c> enumeration.
    /// </summary>
    /// <param name="name">A <c>FunctionName</c> enumeration value representing the name of the current function.</param>
    /// <returns>A <c>FunctionName</c> enumeration value representing the name of the next function following the one given in parameter.</returns>
    public static FunctionName GetNextFunctionName(FunctionName name) => (int)name < functions.Length - 1 ? name + 1 : 0;

    /// <summary>
    /// This function is used to randomly get a function name got from the list of defined function in the <c>FunctionName</c> enumeration.
    /// The function avoid selecting the function given in parameter.
    /// </summary>
    /// <param name="name">A <c>FunctionName</c> enumeration value representing a function to be discarded by the selection processus.</param>
    /// <returns>A <c>FunctionName</c> enumeration value representing the name of he randomly selected function.</returns>
    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
    {
        FunctionName choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0 : choice;
    }

    public static int FunctionCount => functions.Length;

    /// <summary>
    /// This function is used to compute the point position transitions between two given functions.
    /// </summary>
    /// <param name="u">A float value representing the u position of the given point.</param>
    /// <param name="v">A float value representing the v position of the given point.</param>
    /// <param name="t">A float value representing the time at the beginning of this frame.</param>
    /// <param name="from">A delegate instance representing the current function displayed by the graph.
    /// <param name="to">A delegate instance representing the targeted function to display by the graph.
    /// <param name="progress"> A float value representing the duration of the function transition.
    /// <returns>A Unity <c>Vector3</c> structure representing the computed cardinal coordinates of the given point.</returns>

    public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
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
        float r = 0.9f + 0.1f * Sin(PI * (12f * u + 8f * v + t));
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
        float r1 = 0.7f + 0.1f * Sin(PI * (8f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (16f * u + 8f * v + 3f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    #endregion
}
