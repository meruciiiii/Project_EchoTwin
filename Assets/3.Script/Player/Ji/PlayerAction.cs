using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private PlayerEquipment Equipment;
    private IWeaponCommand command;
    private AttackContext context;


    private void Awake()
    {
        Equipment = new PlayerEquipment();
        context = new AttackContext();
    }

    public void OnAttack()
    {
        command?.execute();
    }

    public void OnChargingAttack()
    {

    }

    public void OnCurse()
    {

    }



    public void OnWeaponAcquire(WeaponAbstract newWeapon)
    {
        Equipment.EquipWeapon(newWeapon);
        RebuildAttackCmd();
    }

    private void RebuildAttackCmd()
    {
        AttackCommand mainAttack = new AttackCommand(Equipment.MainWeapon, context);

        if(Equipment.SubWeapon == null)
        {
            command = mainAttack;
        }
        else
        {
            OnEchoCommand subEcho = new OnEchoCommand(Equipment.SubWeapon, context);
            command = new ComboAttackCommand(mainAttack, subEcho, this);
        }
    }
}
