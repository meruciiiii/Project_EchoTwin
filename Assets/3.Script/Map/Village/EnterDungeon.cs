
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    [SerializeField] private StageFlowManager stageFlowManager;
    [SerializeField] private SceneTransition sceneTransition;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SoundManager.SendEvent(SoundType.SFX_Portal);
            sceneTransition.PlayFullTransition(() => {
                stageFlowManager.StartStage();
            });
    }
}
