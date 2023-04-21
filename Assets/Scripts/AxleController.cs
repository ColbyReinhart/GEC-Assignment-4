using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A useful helper class for managing the wheels on a vehicle
[System.Serializable]
public class AxleController : MonoBehaviour
{
    // Editor globals
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool isPowered;
    public bool hasSteering;
    public bool hasHandbrake;

    // Private members
    private Vector3 initialLeftRot;
    private Vector3 initialRightRot;

    private void Awake()
    {
        // Record initial transforms. Useful for steering functionality.
        initialLeftRot = leftWheel.transform.localEulerAngles;
        initialRightRot = rightWheel.transform.localEulerAngles;
    }

    public void DeliverPower(float torque)
    {
        if (!isPowered) { return; }

        leftWheel.motorTorque = torque;
        rightWheel.motorTorque = torque;
    }

    public void Steer(float angle)
    {
        if (!hasSteering) { return; }

        // Physically turn wheel models
        leftWheel.transform.localEulerAngles
            = initialLeftRot + new Vector3(0, angle, 0);
        rightWheel.transform.localEulerAngles
            = initialRightRot + new Vector3(0, angle, 0);

        // Turn the wheel colliders
        leftWheel.steerAngle = angle;
        rightWheel.steerAngle = angle;
    }

    public void ApplyBrake(float torque)
    {
        leftWheel.brakeTorque = torque;
        rightWheel.brakeTorque = torque;
    }

    public void ApplyHandbrake(float torque)
    {
        if (!hasHandbrake) { return; }
        if (torque > leftWheel.brakeTorque) { leftWheel.brakeTorque = torque; }
        if (torque > rightWheel.brakeTorque) { rightWheel.brakeTorque = torque; }
    }
}
