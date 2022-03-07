using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
        private const float HoursToDegree = -30f;
        private const float MinutesToDegree = -6f;
        private const float SecondsToDegree = -6f;
        
        [SerializeField]
        private Transform hoursPivot;

        [SerializeField] 
        private Transform minutesPivot;

        [SerializeField] 
        private Transform secondsPivot;

        private void Update()
        {
                TimeSpan time = DateTime.Now.TimeOfDay;
                hoursPivot.localRotation = Quaternion.Euler(0f, 0f, HoursToDegree * (float) time.TotalHours);
                minutesPivot.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegree * (float) time.TotalMinutes);
                secondsPivot.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegree * (float) time.TotalSeconds);
        }
}
