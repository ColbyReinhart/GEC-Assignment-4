using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls checkpoints throughout the course
public class Checkpoint : MonoBehaviour
{
    private CourseController controller = null;
    private Transform spawnPoint;

    public void SetController(CourseController controller)
    {
        this.controller = controller;
    }

    private void Awake()
    {
        spawnPoint = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        VehicleController player = other.GetComponent<VehicleController>();
        if (player != null)
        {
            bool counted = controller.Notify(this);
            if (counted)
            {
                player.SetSpawn(spawnPoint);
            }
        }
    }
}
