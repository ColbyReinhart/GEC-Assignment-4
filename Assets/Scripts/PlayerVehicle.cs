using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Axle
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool hasMotor;
    public bool hasSteering;

    public void ApplyMotor(float torque)
    {
        leftWheel.motorTorque = torque;
        rightWheel.motorTorque = torque;
    }

    public void ApplyBrake(float torque)
    {
        leftWheel.brakeTorque = torque;
        rightWheel.brakeTorque = torque;
    }

    public void ApplySteering(float steerAngle)
    {
        Vector3 wheelEulers = leftWheel.transform.localEulerAngles;
        wheelEulers.y = steerAngle;
        leftWheel.transform.localEulerAngles = wheelEulers;
        wheelEulers.y = steerAngle + 180f;
        rightWheel.transform.localEulerAngles = wheelEulers;
        leftWheel.steerAngle = steerAngle;
        rightWheel.steerAngle = steerAngle;
    }

    public void ApplyHandbrake(float torque)
    {
        if (torque > leftWheel.brakeTorque)
        {
            leftWheel.brakeTorque = torque;
        }
        if (torque > rightWheel.brakeTorque)
        {
            rightWheel.brakeTorque = torque;
        }

        WheelFrictionCurve curve = leftWheel.sidewaysFriction;
        curve.stiffness = torque > 0 ? 0.5f : 2f;
        leftWheel.sidewaysFriction = curve;
        rightWheel.sidewaysFriction = curve;
    }
}

public class PlayerVehicle : MonoBehaviour
{
    public Axle frontAxle;
    public Axle rearAxle;
    public float motorTorque;
    public float brakeTorque;
    public float handbrakeTorque;
    public float maxSteeringAngle;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        rearAxle.ApplyMotor(Input.GetAxis("Gas") * motorTorque);
        frontAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);
        rearAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);
        frontAxle.ApplySteering(Input.GetAxis("Steering") * maxSteeringAngle);
        rearAxle.ApplyHandbrake(Input.GetAxis("Handbrake") * handbrakeTorque);
    }
}
