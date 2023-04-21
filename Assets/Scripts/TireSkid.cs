using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls drifting sound and functionality
public class TireSkid : MonoBehaviour
{
    public float slipThreshold;

    private WheelCollider wheel;
    private AudioSource skidAudio;
    private ParticleSystem smokeParticles;
    private TrailRenderer skidTrail;

    private bool drifting = false;

    private void Awake()
    {
        wheel = GetComponent<WheelCollider>();
        skidAudio = GetComponent<AudioSource>();
        smokeParticles = GetComponent<ParticleSystem>();
        skidTrail = transform.GetChild(0).GetComponent<TrailRenderer>();
    }

    private void StartDrifting()
    {
        drifting = true;
        skidAudio.Play();
        smokeParticles.Play();
        skidTrail.emitting = true;
    }

    private void StopDrifting()
    {
        drifting = false;
        skidAudio.Stop();
        smokeParticles.Stop();
        skidTrail.emitting = false;
    }

    private void Update()
    {
        WheelHit hit = new WheelHit();

        if (wheel.GetGroundHit(out hit))
        {
            if (Mathf.Abs(hit.sidewaysSlip) > slipThreshold && !drifting)
            {
                StartDrifting();
            }
            else if (Mathf.Abs(hit.sidewaysSlip) < slipThreshold && drifting)
            {
                StopDrifting();
            }
        }
        else if (drifting)
        {
            StopDrifting();
        }
    }
}
