using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;
    public TMP_Text countDownText;
    public AudioClip getReadyBeep;
    public AudioClip startBeep;

    public string timerFormat = "mm':'ss'.'f";

    private AudioSource menuAudio;

    private bool raceStarted = false;
    private float currentLapStamp = 0f;
    private float bestLapTime = 0f;

    private void Awake()
    {
        menuAudio = GetComponent<AudioSource>();
    }

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

    public IEnumerator DoCountdown(CourseController controller)
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
