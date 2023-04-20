using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    public Axle frontAxle;
    public Axle rearAxle;
    public float maxEngineTorque;
    public float brakeTorque;
    public float maxSteeringAngle;
    public float maxSpeed;
    public float baseDownforce;
    public float maxDownforceModifier;

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
        // Prelim stuff
        float maxSpeedPercent = rb.velocity.magnitude / maxSpeed;

        // Calculate engine torque and apply to appropriate axle
        float engineTorque = maxEngineTorque - (maxSpeedPercent * maxEngineTorque);
        rearAxle.DeliverPower(Input.GetAxis("Gas") * engineTorque);

        // Calculate and apply downforce
        rb.AddForce(Vector3.down * (baseDownforce + (maxDownforceModifier * maxSpeedPercent)));

        // Do steering
        frontAxle.Steer(Input.GetAxis("Steering") * maxSteeringAngle);

        // Do brakes
        frontAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);
        rearAxle.ApplyBrake(Input.GetAxis("Brake") * brakeTorque);

        // Do handbrake
        frontAxle.ApplyHandbrake(Input.GetAxis("Handbrake"));
        rearAxle.ApplyHandbrake(Input.GetAxis("Handbrake"));
    }
}
