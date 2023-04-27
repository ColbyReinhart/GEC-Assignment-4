using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceUI : UIController
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;
    public TMP_Text lapNumberText;
    public string timerFormat = "mm':'ss'.'f";

    private float currentLapStamp = 0f;
    private float bestLapTime = 0f;

    protected override void Start()
    {
        base.Start();

        // Load the recorded best lap
        string prefsName = SceneManager.GetActiveScene().name + "BestTime";
        bestLapTime = PlayerPrefs.GetFloat(prefsName);
        bestLapText.text =
                "Best: " + TimeSpan.FromSeconds(bestLapTime).ToString(timerFormat);
    }

    public override void Lap(int newLapNum)
    {
        // Setup the lap counter
        lapNumberText.text =
            newLapNum.ToString() + " of " + CourseController.instance.laps;

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
            // Set the new best lap time
            bestLapTime = lapTime;
            bestLapText.text =
                "Best: " + TimeSpan.FromSeconds(lapTime).ToString(timerFormat);

            // Save it in playerprefs
            string prefsName = SceneManager.GetActiveScene().name + "BestTime";
            PlayerPrefs.SetFloat(prefsName, bestLapTime);
        }
    }

    private void Update()
    {
        HandleMinimap();

        if (!raceStarted) { return; }

        float timeSinceLapStart = Time.time - currentLapStamp;
        currentLapText.text =
            "Lap: " + TimeSpan.FromSeconds(timeSinceLapStart).ToString(timerFormat);
    }
}
