using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public CourseController controller;
    public float checkpointReachDistance = 100f;

    private NavMeshAgent nav;
    private List<Checkpoint> checkpoints;

    private int targetCheckpointIndex = 0;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        checkpoints = controller.GetCheckpoints();
        nav.SetDestination(checkpoints[targetCheckpointIndex].transform.position);
        nav.isStopped = true;
    }

    public void DoDriving(bool value)
    {
        nav.isStopped = !value;
    }

    private void Update()
    {
        Checkpoint targetCheckpoint = checkpoints[targetCheckpointIndex];
        float checkpointDistance =
            (targetCheckpoint.transform.position - transform.position).sqrMagnitude;

        if (checkpointDistance < checkpointReachDistance)
        {
            Debug.Log("Made it to " + targetCheckpoint.gameObject.name);
            targetCheckpointIndex = (targetCheckpointIndex + 1) % checkpoints.Count;
            nav.SetDestination(checkpoints[targetCheckpointIndex].transform.position);
            Debug.Log("Now going to " + checkpoints[targetCheckpointIndex].gameObject.name);
        }
    }
}
