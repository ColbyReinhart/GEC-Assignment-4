using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    //
    // Editor globals
    //

    public List<AxleController> axles;
    public AudioSource crashSound;

    [Space(10)]
    public float baseEngineTorque;
    public float maxSpeed;
    public float maxSteeringAngle;

    [Space(10)]
    public float brakeTorque;
    public float handbrakeTorque;
    public float crashVelocity;

    [Space(10)]
    public int gears;
    public float engineIdlePitch;
    public float engineLowPitch;
    public float engineHighPitch;

    //
    // Component references
    //

    private Rigidbody rb;
    private AudioSource engineAudio;
    private Transform spawnPoint;

    //
    // Private members
    //

    private bool canDrive = false;

    private void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
        spawnPoint = transform;
    }

    public void SetSpawn(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void DoDriving(bool value)
    {
        canDrive = value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= crashVelocity)
        {
            crashSound.Play();
        }
    }

    private void Update()
    {
        // How close are we to max speed?
        float speedPercent = rb.velocity.magnitude / maxSpeed;

        // Decrease engine torque as we approach max speed
        float engineTorque = baseEngineTorque - (speedPercent * baseEngineTorque);

        // Get some information about the current speed/gear
        float speedRangePerGear = (int)maxSpeed / gears;
        int currentGear = (int)(rb.velocity.magnitude / speedRangePerGear); 
        float currentGearSpeed = rb.velocity.magnitude % speedRangePerGear;
        float maxGearSpeedRatio = currentGearSpeed / speedRangePerGear;

        // Calculate the engine audio pitch
        float lowerPitch = currentGear > 0 ? engineLowPitch : engineIdlePitch;
        float pitchRange = engineHighPitch - lowerPitch;
        engineAudio.pitch = (maxGearSpeedRatio * pitchRange) + lowerPitch;

        // Don't do anything past this if we can't drive
        if (!canDrive) { return; }

        // Calculate input values
        float torqueInput = Input.GetAxis("Gas") * engineTorque;
        float steerInput = Input.GetAxis("Steering") * maxSteeringAngle;
        float brakeInput = Input.GetAxis("Brake") * brakeTorque;
        float handbrakeInput = Input.GetAxis("Handbrake") * handbrakeTorque;

        // Handle user input
        foreach (AxleController axle in axles)
        {
            if (axle.isPowered) { axle.DeliverPower(torqueInput); }
            if (axle.hasSteering) { axle.Steer(steerInput); }
            axle.ApplyBrake(brakeInput);
            if (axle.hasHandbrake) { axle.ApplyHandbrake(handbrakeInput); }
        }

        // Reset the car if the user tells us to
        if (Input.GetButtonDown("Respawn"))
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
