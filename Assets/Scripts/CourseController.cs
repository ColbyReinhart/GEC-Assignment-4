using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaceMode { GrandPrix, TimeAttack, ScoreAttack }

public class CourseController : MonoBehaviour
{
    public static CourseController instance;

    [Header("UI Instances")]
    public UIController raceUI;
    public UIController scoreAttackUI;

    [Header("Track Info")]
    public Checkpoint finishLine;
    public List<Checkpoint> checkpoints;
    public List<GameObject> opponents;
    public Transform playerSpawn;
    public List<GameObject> playerPrefabs = new List<GameObject>();
    public int laps = 3;

    [Header("Audio Control")]
    public AudioClip victoryAudio;

    [Header("Misc")]
    public GameObject scoreAttackRoot;

    [NonSerialized]
    private AudioSource levelAudio;
    [NonSerialized]
    public List<Vehicle> vehicles;
    private UIController ui;
    private RaceMode raceMode;
    private bool raceIsOver = false;

    private void Awake()
    {
        // Setup singleton instance, but let it reset itself on scene reloads
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        // Enable mode objects as necessary
        string mode = PlayerPrefs.GetString("SelectedMode", "GrandPrix");
        if (mode == "ScoreAttack")
        {
            raceMode = RaceMode.ScoreAttack;
            ui = scoreAttackUI;
            scoreAttackRoot.SetActive(true);
        }
        else
        {
            ui = raceUI;

            if (mode == "GrandPrix")
            {
                raceMode = RaceMode.GrandPrix;

                foreach (GameObject opponent in opponents)
                {
                    opponent.SetActive(true);
                }
            }
            else
            {
                raceMode = RaceMode.TimeAttack;
            }
        }

        ui.gameObject.SetActive(true);

        // Spawn the player car
        string prefabToUse = PlayerPrefs.GetString("SelectedCar");
        foreach (GameObject car in playerPrefabs)
        {
            if (car.name == prefabToUse)
            {
                Instantiate(car, playerSpawn.position, playerSpawn.rotation);
            }
        }

        // Get all checkpoints
        checkpoints = new List<Checkpoint>(GetComponentsInChildren<Checkpoint>(true));
        checkpoints.Insert(0, finishLine);

        // Get all vehicles
        vehicles =new List<Vehicle>
        (
            UnityEngine.Object.FindObjectsByType<Vehicle>(FindObjectsSortMode.None)
        );

        // Get component references
        levelAudio = GetComponent<AudioSource>();

        // Tell the UI manager to start the countdown
        StartCoroutine(ui.DoCountdown());
    }

    public void DoRace(bool enable)
    {
        // Enable player and CPUs
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.DoDriving(enable);
        }

        // Start level music
        levelAudio.Play();
    }

    // Lets a checkpoint notify the controller when it's been passed through
    // Returns a boolean, if true then the checkpoint should set spawn
    public bool Notify(Checkpoint checkpoint, Vehicle vehicle)
    {
        int checkpointNumber = checkpoints.IndexOf(checkpoint);

        // We only care if we passed the checkpoint after the last one we passed
        if (checkpointNumber == (vehicle.currentCheckpoint + 1) % checkpoints.Count)
        {
            // Set the new current checkpoint
            vehicle.currentCheckpoint = checkpointNumber;

            // If it's the finish line, do a lap
            if (checkpointNumber == 0)
            {
                ++vehicle.currentLap;

                // If it's the player, update the UI
                if (vehicle.CompareTag("Player"))
                {
                    ui.Lap(vehicle.currentLap);
                }

                // If they've finished, end the race
                if (vehicle.currentLap > laps && !raceIsOver)
                {
                    raceIsOver = true;
                    EndRace(vehicle);
                }
            }

            return true;
        }

        return false;
    }

    public List<Checkpoint> GetCheckpoints()
    {
        return checkpoints;
    }

    private void EndRace(Vehicle vehicle)
    {
        // Play the victory audio
        levelAudio.Stop();
        levelAudio.PlayOneShot(victoryAudio);

        // Stop the racers
        foreach (Vehicle racer in vehicles)
        {
            racer.DoDriving(false);
        }

        // Disable all cameras except the main one
        foreach (Camera camera in Camera.allCameras)
        {
            if (camera != Camera.main)
            {
                camera.gameObject.SetActive(false);
            }
            else
            {
                camera.gameObject.SetActive(true);
                camera.enabled = true;
            }
        }

        // Turn off camera stabilization, if necessary
        if (vehicle.CompareTag("Player"))
        {
            vehicle.cameraMount.GetComponent<Stabilize>().enabled = false;
        }

        // Do the victory cutscene on the winning car
        StartCoroutine(vehicle.DoVictoryCutscene());

        // Do race end UI
        bool playerWon = raceMode == RaceMode.ScoreAttack ||
            raceMode == RaceMode.TimeAttack ||
            (raceMode == RaceMode.GrandPrix && vehicle.CompareTag("Player"));
        ui.DoFinishUI(playerWon);

        // Add cash to player prefs if the player won
        if (playerWon)
        {
            int cash = PlayerPrefs.GetInt("Cash");
            PlayerPrefs.SetInt("Cash", cash + 100);
        }
    }
}
