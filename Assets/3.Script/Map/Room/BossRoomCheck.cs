
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomCheck : MonoBehaviour
{
    [SerializeField] private PortalController portalController;
    public void BossIsDead()
    {
        Debug.Log("portal set");
        PortalActive();
    }
    private void PortalActive()
    {
        portalController.gameObject.SetActive(true);
    }
}
