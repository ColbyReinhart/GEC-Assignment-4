using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : Vehicle
{
    public float checkpointReachDistance = 100f;

    private NavMeshAgent nav;
    private int targetCheckpointIndex = 0;

    private new void Awake()
    {
        base.Awake();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = maxSpeed;
    }

    private void Start()
    {
        Checkpoint destination =
            CourseController.instance.checkpoints[targetCheckpointIndex]; 
        nav.SetDestination(destination.transform.position);
        DoDriving(false);
    }

    public override void DoDriving(bool value)
    {
        nav.isStopped = !value;
    }

    private void Update()
    {
        // Calculate pitch of engine noise
        DoEngineNoise();

        // Handle pathfinding
        Checkpoint targetCheckpoint =
            CourseController.instance.checkpoints[targetCheckpointIndex];
        float checkpointDistance =
            (targetCheckpoint.transform.position - transform.position).magnitude;

        if (checkpointDistance < checkpointReachDistance)
        {
            int numCheckpoints = CourseController.instance.checkpoints.Count;
            targetCheckpointIndex = (targetCheckpointIndex + 1) % numCheckpoints;
            StartCoroutine(RouteToNextCheckpoint());
        }
    }

    private IEnumerator RouteToNextCheckpoint()
    {
        Checkpoint destination = CourseController.instance.checkpoints[targetCheckpointIndex];
        Vector3 currentVelocity = nav.velocity;
        nav.SetDestination(destination.transform.position);

        while (nav.pathPending)
        {
            nav.velocity = currentVelocity;
            yield return new WaitForEndOfFrame();
        }
    }
}
