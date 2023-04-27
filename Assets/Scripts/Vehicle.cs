using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// A class representing a vehicle object, whether that be the player or a CPU
public abstract class Vehicle : MonoBehaviour
{
    public AudioSource engineAudio;
    public AudioSource crashAudio;
    public Transform cameraMount;

    [NonSerialized]
    public int currentLap = 0;
    [NonSerialized]
    public int currentCheckpoint = -1;

    public float maxSpeed;
    public float gears;
    public float engineIdlePitch;
    public float engineLowPitch;
    public float engineHighPitch;
    public float crashVelocity;

    protected Rigidbody rb;
    protected Transform spawnPoint;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnPoint = transform;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= crashVelocity)
        {
            crashAudio.Play();
        }
    }

    public abstract void DoDriving(bool value);

    public void SetSpawn(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void Respawn()
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public IEnumerator DoVictoryCutscene()
    {
        Camera.current.transform.parent = cameraMount;
        Camera.current.transform.localPosition = new Vector3(0, 5, 10);
        Camera.current.transform.localEulerAngles = new Vector3(30, 180, 0);
        Vector3 victoryRotation = new Vector3(0, 45, 0);

        while (true)
        {
            cameraMount.Rotate(victoryRotation * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    protected void DoEngineNoise()
    {
        float speedRangePerGear = (int)maxSpeed / gears;
        int currentGear = (int)(rb.velocity.magnitude / speedRangePerGear);
        float currentGearSpeed = rb.velocity.magnitude % speedRangePerGear;
        float maxGearSpeedRatio = currentGearSpeed / speedRangePerGear;

        // Calculate the engine audio pitch
        float lowerPitch = currentGear > 0 ? engineLowPitch : engineIdlePitch;
        float pitchRange = engineHighPitch - lowerPitch;
        engineAudio.pitch = (maxGearSpeedRatio * pitchRange) + lowerPitch;
    }
}
