
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private List<PortalController> portalControllers;
    public event Action onPortalEntered;
    private void Awake()
    {
        foreach(PortalController potal in portalControllers)
        {
            potal.onPortalEntered += OnPortalEntered;
        }
    }
    private void OnPortalEntered()
    {
        onPortalEntered?.Invoke();
    }
}
