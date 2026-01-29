using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingAttackCommand : IWeaponCommand
{
    private WeaponAbstract weapon;

    public ChargingAttackCommand(WeaponAbstract weapon)
    {
        this.weapon = weapon;
    }

    public void execute()
    {
        weapon.ChargingAttack();
    }
}
