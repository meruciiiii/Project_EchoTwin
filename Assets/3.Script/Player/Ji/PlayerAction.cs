using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FlashEffect))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerEquipment Equipment;
    private InputManager inputManager;
    private IWeaponCommand command;
    private AttackContext context;
    private PlayerStats stats;
    private FlashEffect effect;

    private ItemPickup currentPickup;

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
        TryGetComponent(out inputManager);
        TryGetComponent(out gizmo);
    }

    private void Update()
    {
        if (stats.isDash || Equipment.MainWeapon == null) return;

        if(inputManager.isAttackPressed)
        {
            if(Equipment.MainWeapon.CanAttack())
            {
                OnAttack();
            }
        }
    }

    public void OnAttack()
    {
        context = new AttackContext();
        RebuildAttackCmd();
        command?.execute();
    }

    public void OnChargingAttack()
    {

    }

    public void OnCurse()
    {

    }

    public void SetPickup(ItemPickup pickup)
    {
        currentPickup = pickup;
    }

    public void ClearPickup(ItemPickup pickup)
    {
        if (currentPickup == pickup) currentPickup = null;
    }

    public void GetItem()
    {
        if (currentPickup == null) return;
        currentPickup.GetNewWeapon();
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
        Equipment.EquipWeapon(newWeapon);

        if (gizmo.mainWeapon == null)
        {
            gizmo.mainWeapon = newWeapon;//gizmo
        }
        else
        {
            gizmo.subWeapon = gizmo.mainWeapon;
            gizmo.mainWeapon = newWeapon;
        }
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
