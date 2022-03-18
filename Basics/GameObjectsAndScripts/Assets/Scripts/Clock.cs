using System;
using UnityEngine;

/// <summary>
/// Class <c>Clock</c> is a Unity component script used to manage the clock game object general behavior.
/// </summary>
public class Clock : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Constant variable <c>HoursToDegree</c> represents the angle value associated with one hour of the hour hand of a wall clock. 
    /// </summary>
    private const float HoursToDegree = -30f;

    /// <summary>
    /// Constant variable <c>MinutesToDegree</c> represents the angle value associated with one minute of the minute hand of a wall clock. 
    /// </summary>
    private const float MinutesToDegree = -6f;

    /// <summary>
    /// Constant variable <c>SecondsToDegree</c> represents the angle value associated with one seconds of the seconds hand of a wall clock. 
    /// </summary>
    private const float SecondsToDegree = -6f;

    /// <summary>
    /// Instance variable <c>hoursPivot</c> is a Unity <c>Transform</c> structure representing position, rotation and scale of the hour hand pivot point of the clock.
    /// </summary>
    [SerializeField]
    private Transform hoursPivot;

    /// <summary>
    /// Instance variable <c>minutesPivot</c> is a Unity <c>Transform</c> structure representing position, rotation and scale of the minute hand pivot point of the clock.
    /// </summary>
    [SerializeField]
    private Transform minutesPivot;

    /// <summary>
    /// Instance variable <c>secondsPivot</c> is a Unity <c>Transform</c> structure representing position, rotation and scale of the second hand pivot point of the clock.
    /// </summary>
    [SerializeField]
    private Transform secondsPivot;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Use timespan to continuously rotate the clock hands.
        TimeSpan time = DateTime.Now.TimeOfDay;
        hoursPivot.localRotation = Quaternion.Euler(0f, 0f, HoursToDegree * (float)time.TotalHours);
        minutesPivot.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegree * (float)time.TotalMinutes);
        secondsPivot.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegree * (float)time.TotalSeconds);
    }

    #endregion
}
