using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>FrameRateCounter</c> is a Unity script used to manager the frame rate counter game object behavior.
/// </summary>
public class FrameRateCounter : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>display</c> is a Unity <c>TextMeshProUGUI</c> component representing the text UI manager of the frame rate counter.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI display;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    public enum DisplayMode { FPS, MS }

    /// <summary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private DisplayMode displayMode = DisplayMode.FPS;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    [SerializeField, Range(0.1f, 2f)]
    private float sampleDuration = 1f;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private int frames;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private float duration, bestDuration = float.MaxValue, worstDuration;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;

        if (frameDuration < bestDuration) {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration) {
            worstDuration = frameDuration;
        }

        if (duration >= sampleDuration) {
            if (displayMode == DisplayMode.FPS) {
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 1f / bestDuration, frames / duration, 1f / worstDuration);
            } else {
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000f * bestDuration, 1000f * duration / frames, 1000f * worstDuration);
            }
            frames = 0;
            duration = 0f;
            bestDuration = float.MaxValue;
            worstDuration = 0f;
        }
    }
}
