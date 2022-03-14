using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>FrameRateCounter</c> is a Unity script used to manager the frame rate counter game object behavior.
/// </summary>
public class FrameRateCounter : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance variable <c>display</c> is a Unity <c>TextMeshProUGUI</c> component representing the text UI manager of the frame rate counter.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI display;

    /// <summary>
    /// Enumeration variable <c>DisplayMode</c> representing the different performance display mode names available.
    /// </summary>
    public enum DisplayMode { FPS, MS }

    /// <summary>
    /// Instance variable <c>displayMode</c> is a <c>DisplayMode</c> enumeration instance representing the current performent display mode.
    /// </summary>
    [SerializeField]
    private DisplayMode displayMode = DisplayMode.FPS;

    /// <summary>
    /// Instance variable <c>sampleDuration</c> represents the acquisition duration value of a sample of frames.
    /// </summary>
    [SerializeField, Range(0.1f, 2f)]
    private float sampleDuration = 1f;

    /// <summary>
    /// Instance variable <c>frames</c> represents the number value of frames in the sample.
    /// </summary>
    private int frames;

    /// <summary>
    /// Instance variable <c>duration</c> represents the elapsed time value since the last sample of frames acquisition.
    /// </summary>
    private float duration;

    /// <summary>
    /// Instance variable <c>worstDuration</c> represents the worst value of a frame duration (worst = longest frame duration).
    /// </summary>
    private float worstDuration;

    /// <summary>
    /// Instance variable <c>bestDuration</c> represents the best value of a frame duration (best = shortest frame duration).
    /// </summary>
    private float bestDuration = float.MaxValue;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;

        if (frameDuration < bestDuration)
        {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration)
        {
            worstDuration = frameDuration;
        }

        if (duration >= sampleDuration)
        {
            if (displayMode == DisplayMode.FPS)
            {
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 1f / bestDuration, frames / duration, 1f / worstDuration);
            }
            else
            {
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000f * bestDuration, 1000f * duration / frames, 1000f * worstDuration);
            }
            frames = 0;
            duration = 0f;
            bestDuration = float.MaxValue;
            worstDuration = 0f;
        }
    }

    #endregion
}