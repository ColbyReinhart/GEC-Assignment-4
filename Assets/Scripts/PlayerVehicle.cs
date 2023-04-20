using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    public Axle frontAxle;
    public Axle rearAxle;
    public float maxEngineTorque;
    public float brakeTorque;
    public float handbrakeTorque;
    public float maxSteeringAngle;
    public float maxSpeed;
    public int gears = 5;
    public float gearIdlePitch = 0.3f;
    public float gearLowPitch = 1f;
    public float gearHighPitch = 2f;

    private Rigidbody rb;
    private AudioSource engineAudio;
    private float speedPerGearMod;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();
        speedPerGearMod = maxSpeed / gears;
    }

    // Finds the corresponding visual wheel and correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        Transform wheelObject = collider.transform;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        wheelObject.transform.position = position;
        wheelObject.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        // Prelim stuff
        float maxSpeedPercent = rb.velocity.magnitude / maxSpeed;

        // Calculate engine torque and apply to appropriate axle
        float engineTorque = maxEngineTorque - (maxSpeedPercent * maxEngineTorque);
        rearAxle.DeliverPower(Input.GetAxis("Gas") * engineTorque);

        // Do steering
        frontAxle.Steer(Input.GetAxis("Steering") * maxSteeringAngle);

        // Do brakes
        frontAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);
        rearAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);

        // Do handbrake
        rearAxle.ApplyHandbrake(Input.GetAxis("Handbrake") * handbrakeTorque);

        // Handle engine audio

        // What gear are we in?
        int gear = (int)(rb.velocity.magnitude / speedPerGearMod);

        // If we're in 0th gear, the lowest pitch in the range should
        // be lower to make the engine sound like it's idling
        float lowPitch = (gear > 0) ? gearLowPitch : gearIdlePitch;

        // Find how fast we are in the current gear, then find the
        // "fast percentage" out of this gear.
        float currentGearSpeed = rb.velocity.magnitude % speedPerGearMod;
        float currentGearSpeedRatio = currentGearSpeed / speedPerGearMod;

        // Find the engine audio pitch based on what we found above
        float pitchRange = gearHighPitch - lowPitch;
        float pitch = (currentGearSpeedRatio * pitchRange) + lowPitch;
        engineAudio.pitch = pitch;
    }
}
