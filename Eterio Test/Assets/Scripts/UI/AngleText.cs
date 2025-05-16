using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleText : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 toCamera = Camera.main.transform.position - transform.position;
        toCamera.z = 0;
        toCamera.x = 0;
        transform.rotation = Quaternion.LookRotation(-toCamera);
        transform.position = transform.position + toCamera.normalized / 5f;
    }
}
