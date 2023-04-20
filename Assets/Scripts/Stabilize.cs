using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilize : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, transform.parent.eulerAngles.y, 0);
    }
}
