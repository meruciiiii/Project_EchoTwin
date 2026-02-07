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
    private void Awake()
    {
        if (!TryGetComponent(out storeController))
            Debug.Log("TryGetComponent StoreController is fail");
        if (!TryGetComponent(out storeUIController))
            Debug.Log("TryGetComponent StoreController is fail");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            storeEvent += storeUIController.StoreOpen;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            storeEvent -= storeUIController.StoreOpen;
            storeUIController.StoreClose();
        }
    }
}
