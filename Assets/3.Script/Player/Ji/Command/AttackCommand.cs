using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : IWeaponCommand
{
    private WeaponAbstract weapon;
    private AttackContext context;

    public AttackCommand(WeaponAbstract weapon, AttackContext context)
    {
        this.weapon = weapon;
        this.context = context;
    }

    public void execute()
    {
        if (!weapon.canCombo()) return;

        context.hitTargets.Clear();
        weapon.Attack(context);
        Debug.Log("attackCommand");
    }
}
