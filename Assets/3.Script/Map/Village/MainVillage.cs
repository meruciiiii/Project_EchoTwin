using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainVillage : MonoBehaviour
{
    //이동
    //업그레이드

    //활동 대상 플레이어
    //감지 후 작동
    private StoreController storeController;
    private StoreUIController storeUIController;
    private Action storeEvent;

    //추가
    private PlayerStats stats;

    private void Awake()
    {
        if (!TryGetComponent(out storeController))
            Debug.Log("TryGetComponent StoreController is fail");
        if (!TryGetComponent(out storeUIController))
            Debug.Log("TryGetComponent storeUIController is fail");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //storeEvent += storeUIController.StoreOpen;
            //이벤트 추가 하고 invoke 없음 + 이벤트보다 method 직접 실행

            stats = other.GetComponent<PlayerStats>();
            if(stats == null) Debug.Log("stats null");

            storeUIController.setPlayerStats(stats);
            storeUIController.RefreshUI();
            storeUIController.StoreOpen();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //storeEvent -= storeUIController.StoreOpen;
            //storeUIController.StoreClose();

            storeUIController.clearPlayerStats();
            stats = null;
            storeUIController.StoreClose();
        }
    }
}
