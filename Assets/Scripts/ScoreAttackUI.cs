using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreAttackUI : UIController
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;
    public TMP_Text lapNumberText;

    private List<ScoreAttackToken> tokens;
    private int currentLapScore;
    private int bestLapScore;

    protected override void Start()
    {
        base.Start();

        // Load the recorded best lap
        string prefsName = SceneManager.GetActiveScene().name + "BestScore";
        bestLapScore = PlayerPrefs.GetInt(prefsName);
        bestLapText.text = "Best: " + bestLapScore;

        // Collect all tokens
        tokens =new List<ScoreAttackToken>
            (GameObject.FindObjectsOfType<ScoreAttackToken>());
        foreach (ScoreAttackToken token in tokens)
        {
            token.RegsisterUI(this);
        }
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
            currentLapScore = 0;
            return;
        }

        // Bring back all tokens
        foreach (ScoreAttackToken token in tokens)
        {
            token.gameObject.SetActive(true);
        }

        // Was this the best score?
        if (bestLapScore < currentLapScore)
        {
            // Set the new best lap time
            bestLapScore = currentLapScore;
            bestLapText.text =
                "Best: " + bestLapScore;

            // Save it in playerprefs
            string prefsName = SceneManager.GetActiveScene().name + "BestScore";
            PlayerPrefs.SetInt(prefsName, bestLapScore);
        }
    }

    public void AddPoints(int points)
    {
        currentLapScore += points;
        currentLapText.text = "Lap: " + currentLapScore;
    }

    private void Update()
    {
        HandleMinimap();
    }
}
