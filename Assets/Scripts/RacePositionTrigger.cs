using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacePositionTrigger : MonoBehaviour
{
    public bool isFront;
    private UIController uiController;

    private void Start()
    {
        uiController = GameObject.FindObjectOfType<UIController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiController.UpdatePosition(isFront);
        }
    }
}
