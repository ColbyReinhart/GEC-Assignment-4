using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Axle : MonoBehaviour
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public float forwardSlipThreshold = 0.4f;
    public float sideSlipThreshold = 0.2f;

    private WheelCollider[] wheels;
    private AudioSource skidAudio;
    private bool isDrifting = false;

    private void Awake()
    {
        wheels = new WheelCollider[2];
        wheels[0] = leftWheel;
        wheels[1] = rightWheel;

        skidAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // We'll just check one wheel for efficiency
        WheelHit hit = new WheelHit();
        if (leftWheel.GetGroundHit(out hit))
        {
            if (hit.sidewaysSlip >= sideSlipThreshold||
                hit.forwardSlip >= forwardSlipThreshold)
            {
                if (!isDrifting)
                {
                    skidAudio.Play();

                    foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
                    {
                        particles.Play();
                    }

                    isDrifting = true;
                }
            }
            else
            {
                if (isDrifting)
                {
                    skidAudio.Stop();

                    foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
                    {
                        particles.Stop();
                    }

                    isDrifting = false;
                }
            }
        }
    }

    public void DeliverPower(float torque)
    {
        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = torque;
        }
    }

    public void ApplyBrake(float torque)
    {
        foreach (WheelCollider wheel in wheels)
        {
            wheel.brakeTorque = torque;
        }
    }

    public void Steer(float angle)
    {
        foreach (WheelCollider wheel in wheels)
        {
            // Physically turn the wheel
            Vector3 eulers = wheel.transform.localEulerAngles;
            eulers.y = (wheel == leftWheel) ? angle : 180f + angle;
            wheel.transform.localEulerAngles = eulers;

            // Set steer angle
            wheel.steerAngle = angle;
        }
    }

    public void ApplyHandbrake(float torque)
    {
        foreach (WheelCollider wheel in wheels)
        {
            // Don't do anything unless the handbrake is doing more
            // than what's already applied
            if (torque >  wheel.brakeTorque)
            {
                wheel.brakeTorque = torque;
            }
        }
    }
}
