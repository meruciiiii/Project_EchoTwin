
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    private void Awake()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        mapManager.GenerateMap(0);
    }
}
