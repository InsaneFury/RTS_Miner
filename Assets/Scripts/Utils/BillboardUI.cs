using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    Quaternion originalRotation;
    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation * originalRotation;
    }
}
