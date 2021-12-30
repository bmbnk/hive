using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Hex;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - Hex.transform.position;
    }

    void LateUpdate()
    {
        //transform.position = hex.transform.position + offset;
    }
}
