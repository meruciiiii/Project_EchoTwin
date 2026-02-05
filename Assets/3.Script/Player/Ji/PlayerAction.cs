using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FlashEffect))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerEquipment Equipment;
    private IWeaponCommand command;
    private AttackContext context;
    private PlayerStats stats;
    private FlashEffect effect;

    private bool hasDamaged = false;
    [SerializeField] private float invincibilityTime = 1f;

    [SerializeField] private float knockBackForce = 2f;

    public AttackDebugGizmo gizmo;

    private void Awake()
    {
        if (Equipment == null)
        {
            Equipment = new PlayerEquipment();
        }
        TryGetComponent(out stats);
        TryGetComponent(out effect);
    }

    public void OnAttack()
    {
        if (stats.isDash) return;

        context = new AttackContext();
        RebuildAttackCmd();
        command?.execute();

        Debug.Log("playerAction");
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

    public void takeDamage(int damage,Vector3 damagePos)
    {
        if (hasDamaged) return;

        stats.takeDamage(damage);

        Vector3 dir = (damagePos - transform.position).normalized;
        knockBack(dir);

        StartCoroutine(superArmor());

        effect.Flash(stats.FlashAmount, stats.FlashDuration);

        if (stats.isDead)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Die);
        }
    }

    private void knockBack(Vector3 dir)
    {
        transform.GetComponent<Rigidbody>().AddForce(-dir* knockBackForce);
    }

    public void OnWeaponAcquire(WeaponAbstract newWeapon)
    {
        if (gizmo.mainWeapon == null)
        {
            gizmo.mainWeapon = newWeapon;//gizmo
        }
        else
        {
            gizmo.subWeapon = gizmo.mainWeapon;
            gizmo.mainWeapon = newWeapon;
        }

        Equipment.EquipWeapon(newWeapon);
    }

    private void RebuildAttackCmd()
    {
        AttackCommand mainAttack = new AttackCommand(Equipment.MainWeapon, context);

        if (Equipment.SubWeapon == null)
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
