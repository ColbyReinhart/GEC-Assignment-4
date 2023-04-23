using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    private Camera[] cameras;
    private int currentCamIndex = 0;

    private void Awake()
    {
        cameras = GetComponentsInChildren<Camera>();
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("CameraSwitch"))
        {
            cameras[currentCamIndex].enabled = false;
            currentCamIndex = (currentCamIndex + 1) % cameras.Length;
            cameras[currentCamIndex].enabled = true;
        }
    }
}
