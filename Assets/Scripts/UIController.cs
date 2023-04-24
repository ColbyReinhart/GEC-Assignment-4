using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image minimap;
    public GameObject marker;

    [Space(10)]

    public TMP_Text currentLapText;
    public TMP_Text bestLapText;
    public TMP_Text lapNumberText;
    public TMP_Text countDownText;
    public AudioClip getReadyBeep;
    public AudioClip startBeep;

    public string timerFormat = "mm':'ss'.'f";

    private AudioSource menuAudio;

    private Dictionary<Vehicle, GameObject> minimapMarkers;
    private bool raceStarted = false;
    private float currentLapStamp = 0f;
    private float bestLapTime = 0f;

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

        // Setup minimap markers
        minimapMarkers = new Dictionary<Vehicle, GameObject>();
        foreach (Vehicle vehicle in CourseController.instance.vehicles)
        {
            GameObject minimapMarker = Instantiate(marker, minimap.transform);
            Image image = minimapMarker.GetComponent<Image>();
            image.color = (vehicle.CompareTag("Player")) ? Color.red : Color.blue;
            minimapMarkers.Add(vehicle, minimapMarker);
        }
    }

    public void Lap(int newLapNum)
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
        CourseController.instance.DoRace(true);
        yield return new WaitForSeconds(1);

        // Disable countdown text
        countDownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleMinimap();

        if (!raceStarted) { return; }

        float timeSinceLapStart = Time.time - currentLapStamp;
        currentLapText.text =
            "Lap: " + TimeSpan.FromSeconds(timeSinceLapStart).ToString(timerFormat);
    }

    private void HandleMinimap()
    {
        // Get terrain dimensions
        Vector3 terrainPos = Terrain.activeTerrain.GetPosition();
        Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;

        // Get minimap dimenstions
        RectTransform minimapRect = minimap.GetComponent<RectTransform>();
        Vector2 minimapSize = new Vector2
        (
            minimapRect.sizeDelta.x * minimapRect.localScale.x,
            minimapRect.sizeDelta.y * minimapRect.localScale.y
        );

        foreach (Vehicle vehicle in CourseController.instance.vehicles)
        {

            // Where is the vehicle relative to to terrain
            Vector3 relativePos = vehicle.transform.position - terrainPos;

            // Translate into 2d space and get offset ratio
            Vector2 translatedPos = new Vector2
            (
                relativePos.x / terrainSize.x,
                relativePos.z / terrainSize.z
            );

            // Determine position on minimap
            Vector2 minimapPos = new Vector2
            (
                minimapSize.x * translatedPos.x,
                minimapSize.y * translatedPos.y
            );

            // Position marker on minimap
            RectTransform rect =
                minimapMarkers[vehicle].GetComponent<RectTransform>(); 
            rect.anchoredPosition = minimapPos * -1;
        }
    }
}
