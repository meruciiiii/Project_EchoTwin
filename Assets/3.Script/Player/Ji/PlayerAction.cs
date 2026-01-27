using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private PlayerEquipment Equipment;
    private PlayerStats Stats;

    private void Awake()
    {
        TryGetComponent(out Equipment);
        TryGetComponent(out Stats);
    }

    public void Attack()
    {

    }

    public void ChargingAttack()
    {

    }

    public void Curse()
    {

    }
}
