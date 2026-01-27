using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Weapon & Magic")]
    [Space(5f)]
    [SerializeField] private WeaponData weapon;
    [SerializeField] private CurseData magic;

    private bool isMain = false;
    private bool isSub = false;

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Equipment"))
        {
            //pick UI popup
        }
    }

    private void getEquipment(GameObject target)//UI에서 상호작용 키 눌렀을 때 실행되는 method
    {

    }

}
