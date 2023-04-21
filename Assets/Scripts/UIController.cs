using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;

    public string timerFormat = "mm':'ss'.'f";

    private bool raceStarted = false;
    private float currentLapStamp = 0f;
    private float bestLapTime = 0f;

    public void Lap()
    {
        // Start counting
        if (!raceStarted)
        {
            raceStarted = true;
            currentLapStamp = Time.time;
            return;
        }

        // Calculate lap time
        float lapTime = Time.time - currentLapStamp;
        currentLapStamp = Time.time;

        // Was this the best lap?
        if (lapTime < bestLapTime || bestLapTime == 0f)
        {
            bestLapTime = lapTime;
            bestLapText.text =
                "Best: " + TimeSpan.FromSeconds(lapTime).ToString(timerFormat);
        }
    }

    private void Update()
    {
        if (!raceStarted) { return; }

        float timeSinceLapStart = Time.time - currentLapStamp;
        currentLapText.text =
            "Lap: " + TimeSpan.FromSeconds(timeSinceLapStart).ToString(timerFormat);
    }
}
