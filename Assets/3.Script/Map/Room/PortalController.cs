    
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private bool bossCheck = true;
    public event Action onPortalEntered;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SoundManager.SendEvent(SoundType.SFX_Portal);

        Debug.Log("Portal Triger is started");
        OnPortalEntered();
    }
    private void OnPortalEntered()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Clear);
        onPortalEntered?.Invoke();
    }
}
