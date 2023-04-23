using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public CourseController controller;
    public float checkpointReachDistance = 100f;

    public float gears;
    public float engineIdlePitch;
    public float engineLowPitch;
    public float engineHighPitch;

    private NavMeshAgent nav;
    private AudioSource engineAudio;

    private List<Checkpoint> checkpoints;
    private int targetCheckpointIndex = 0;
    private float maxSpeed;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        engineAudio = GetComponent<AudioSource>();
        maxSpeed = nav.speed;
    }

    private void Start()
    {
        checkpoints = controller.GetCheckpoints();
        nav.SetDestination(checkpoints[targetCheckpointIndex].transform.position);
        nav.isStopped = true;
    }

    public void DoDriving(bool value)
    {
        nav.isStopped = !value;
    }

    private void Update()
    {
        // Copy pasted code from VehicleController (running out of time)
        // Get some information about the current speed/gear
        float speedRangePerGear = (int)(maxSpeed + 5f) / gears;
        int currentGear = (int)(nav.velocity.magnitude / speedRangePerGear); 
        float currentGearSpeed = nav.velocity.magnitude % speedRangePerGear;
        float maxGearSpeedRatio = currentGearSpeed / speedRangePerGear;

        // Calculate the engine audio pitch
        float lowerPitch = currentGear > 0 ? engineLowPitch : engineIdlePitch;
        float pitchRange = engineHighPitch - lowerPitch;
        engineAudio.pitch = (maxGearSpeedRatio * pitchRange) + lowerPitch;

        // Handle pathfinding
        Checkpoint targetCheckpoint = checkpoints[targetCheckpointIndex];
        float checkpointDistance =
            (targetCheckpoint.transform.position - transform.position).sqrMagnitude;

        if (checkpointDistance < checkpointReachDistance)
        {
            Debug.Log("Made it to " + targetCheckpoint.gameObject.name);
            targetCheckpointIndex = (targetCheckpointIndex + 1) % checkpoints.Count;
            nav.SetDestination(checkpoints[targetCheckpointIndex].transform.position);
            Debug.Log("Now going to " + checkpoints[targetCheckpointIndex].gameObject.name);
        }
    }
}
