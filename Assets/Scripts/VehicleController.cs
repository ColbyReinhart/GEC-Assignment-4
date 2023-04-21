using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    //
    // Editor globals
    //

    public List<AxleController> axles;

    [Space(10)]
    public float baseEngineTorque;
    public float maxSpeed;
    public float maxSteeringAngle;

    [Space(10)]
    public float brakeTorque;
    public float handbrakeTorque;

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

    private void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
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
    }
}
