using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEchoCommand : IWeaponCommand
{
    private WeaponAbstract subWeapon;
    private AttackContext context;

    public OnEchoCommand(WeaponAbstract subWeapon, AttackContext context)
    {
        this.subWeapon = subWeapon;
        this.context = context;
    }

    public void execute()
    {
        if (!subWeapon.canEcho()) return;

        Debug.Log(context.hitTargets.Count);
        if (context.hitTargets.Count == 0) return;

        subWeapon.OnEcho(context);
        subWeapon.ConsumeResonance();
        Debug.Log("onecho");
    }
}
