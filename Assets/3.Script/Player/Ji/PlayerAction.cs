using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FlashEffect))]
public class PlayerAction : MonoBehaviour
{
    private PlayerEquipment Equipment;
    private IWeaponCommand command;
    private AttackContext context;
    private PlayerStats stats;
    private FlashEffect effect;

    private bool hasDamaged = false;
    [SerializeField] private float invincibilityTime = 2f;

    private void Awake()
    {
        Equipment = new PlayerEquipment();
        context = new AttackContext();
        TryGetComponent(out stats);
        TryGetComponent(out effect);
    }

    public void OnAttack()
    {
        if (stats.isDash) return;
        command?.execute();
    }

    public void OnChargingAttack()
    {

    }

    public void OnCurse()
    {

    }

    private IEnumerator superArmor()
    {
        hasDamaged = true;
        yield return new WaitForSeconds(invincibilityTime);
        hasDamaged = false;
    }

    public void takeDamage(int damage)
    {
        if (hasDamaged) return;

        stats.takeDamage(damage);
        StartCoroutine(superArmor());

        effect.Flash(stats.FlashAmount,stats.FlashDuration);

        if(stats.isDead)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Die);
        }
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
