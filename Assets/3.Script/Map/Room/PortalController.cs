    
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private string roomTag;
    public event Action onPortalEntered;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Portal Triger is started");
        OnPortalEntered();
    }
    private void OnPortalEntered()
    {
        onPortalEntered?.Invoke();
    }
}
