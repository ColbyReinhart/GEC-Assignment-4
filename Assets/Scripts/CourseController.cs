using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseController : MonoBehaviour
{
    public UIController ui;
    public Checkpoint finishLine;
    public VehicleController playerVehicle;
    public List<VehicleAI> cpus;

    public int laps = 3;

    // All checkpoints except the finish line should be children of this object
    private List<Checkpoint> checkpoints;
    private AudioSource levelAudio;

    private int currentLap = 0;
    private int currentCheckpoint = -1;

    private void Awake()
    {
        // Get all checkpoints
        checkpoints = new List<Checkpoint>(GetComponentsInChildren<Checkpoint>(true));
        checkpoints.Insert(0, finishLine);

        // Register with all checkpoints
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetController(this);
        }

        // Get component references
        levelAudio = GetComponent<AudioSource>();

        // Register with the UI Controller
        ui.SetCourseController(this);

        // Tell the UI manager to start the countdown
        StartCoroutine(ui.DoCountdown());
    }

    public void DoRace(bool enable)
    {
        // Enable player and CPUs
        playerVehicle.DoDriving(enable);
        foreach (VehicleAI ai in cpus)
        {
            ai.DoDriving(enable);
        }

        // Start level music
        levelAudio.Play();
    }

    // Lets a checkpoint notify the controller when it's been passed through
    // Returns a boolean denoting if the checkpoint "counted" or not
    public bool Notify(Checkpoint checkpoint)
    {
        int checkpointNumber = checkpoints.IndexOf(checkpoint);

        // We only care if we passed the checkpoint after the last one we passed
        if (checkpointNumber == (currentCheckpoint + 1) % checkpoints.Count)
        {
            // Set the new current checkpoint
            Debug.Log("Passed checkpoint " + checkpointNumber);
            currentCheckpoint = checkpointNumber;

            // If it's the finish line, do a lap
            if (checkpointNumber == 0)
            {
                ++currentLap;
                ui.Lap(currentLap);
                Debug.Log("Now on lap " + currentLap);

                if (currentLap > laps)
                {
                    EndRace();
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

    private void EndRace()
    {

    }
}
