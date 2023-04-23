using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;
    public TMP_Text lapNumberText;
    public TMP_Text countDownText;
    public AudioClip getReadyBeep;
    public AudioClip startBeep;

    public string timerFormat = "mm':'ss'.'f";

    private AudioSource menuAudio;
    private CourseController controller;

    private bool raceStarted = false;
    private float currentLapStamp = 0f;
    private float bestLapTime = 0f;
    private int currentLap = 0;

    private void Awake()
    {
        menuAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Load the recorded best lap
        string prefsName = SceneManager.GetActiveScene().name + "BestTime";
        bestLapTime = PlayerPrefs.GetFloat(prefsName);
        bestLapText.text =
                "Best: " + TimeSpan.FromSeconds(bestLapTime).ToString(timerFormat);
    }

    public void SetCourseController(CourseController controller)
    {
        this.controller = controller;
    }

    public void Lap()
    {
        // Setup the lap counter
        ++currentLap;
        lapNumberText.text = currentLap.ToString() + " of " + controller.laps;

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

    public IEnumerator DoCountdown()
    {
        // Wait a second for everything to initialize
        yield return new WaitForSeconds(1);

        // Do the countdown
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            menuAudio.PlayOneShot(getReadyBeep);
            yield return new WaitForSeconds(1);
        }

        // Start the race!
        countDownText.text = "GO!";
        menuAudio.PlayOneShot(startBeep);
        controller.DoRace(true);
        yield return new WaitForSeconds(1);

        // Disable countdown text
        countDownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!raceStarted) { return; }

        float timeSinceLapStart = Time.time - currentLapStamp;
        currentLapText.text =
            "Lap: " + TimeSpan.FromSeconds(timeSinceLapStart).ToString(timerFormat);
    }
}
