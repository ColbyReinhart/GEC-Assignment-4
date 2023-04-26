using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : Vehicle
{

    //
    // Editor globals
    //

    public List<AxleController> axles;
    public float baseEngineTorque;
    public float maxSteeringAngle;
    public float brakeTorque;
    public float handbrakeTorque;

    //
    // Private members
    //

    private bool canDrive = false;

    //
    // Methods
    //

    public override void DoDriving(bool value)
    {
        canDrive = value;

        foreach (AxleController axle in axles)
        {
            axle.DeliverPower(0);
            axle.ApplyBrake(brakeTorque);
            axle.ApplyHandbrake(handbrakeTorque);
            axle.Steer(0);
        }
    }

    private void Update()
    {
        // How close are we to max speed?
        float speedPercent = rb.velocity.magnitude / maxSpeed;

        // Decrease engine torque as we approach max speed
        float engineTorque = baseEngineTorque - (speedPercent * baseEngineTorque);

        // Calculate the pitch of the engine noise
        DoEngineNoise();

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
            Respawn();
        }
    }
}
