using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls checkpoints throughout the course
public class Checkpoint : MonoBehaviour
{
    private Transform spawnPoint;

    private void Awake()
    {
        spawnPoint = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Did a player pass the checkpoint?
        Vehicle vehicle = other.GetComponent<Vehicle>();
        if (vehicle != null)
        {
            bool setSpawn = CourseController.instance.Notify(this, vehicle);
            if (setSpawn)
            {
                vehicle.SetSpawn(spawnPoint);
            }
        }
    }
}
