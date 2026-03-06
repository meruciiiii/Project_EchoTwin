
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    [SerializeField] private StageFlowManager stageFlowManager;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        stageFlowManager.StartStage();
    }
}
