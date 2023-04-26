using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseController : MonoBehaviour
{
    public static CourseController instance;

    public UIController ui;
    public Checkpoint finishLine;
    public AudioClip victoryAudio;
    public int laps = 3;

    [Space(10)]

    public Transform playerSpawn;
    public List<GameObject> playerPrefabs = new List<GameObject>();

    // All checkpoints except the finish line should be children of this object
    [NonSerialized]
    public List<Checkpoint> checkpoints;
    private AudioSource levelAudio;
    [NonSerialized]
    public List<Vehicle> vehicles;
    private bool raceIsOver = false;

    private void Awake()
    {
        // Setup singleton instance, but let it reset itself on scene reloads
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

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
    }
}
