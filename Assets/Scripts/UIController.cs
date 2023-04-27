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

    public TMP_Text countDownText;
    public AudioClip getReadyBeep;
    public AudioClip startBeep;

    protected AudioSource menuAudio;
    protected Dictionary<Vehicle, GameObject> minimapMarkers;
    protected bool raceStarted = false;

    protected void Awake()
    {
        menuAudio = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
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

    // What happens on the next lap?
    public virtual void Lap(int newLapNum) { }

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

    protected void HandleMinimap()
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
