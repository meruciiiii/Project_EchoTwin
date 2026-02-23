
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private void Start()
    {
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }
    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
