using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(FlashEffect))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] public PlayerEquipment Equipment;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    public Transform RightHand => rightHand;
    public Transform LeftHand => leftHand;
    private InputManager inputManager;
    private IWeaponCommand command;
    private AttackContext context;
    private PlayerStats stats;
    private FlashEffect effect;
    private Animator ani;

    private bool hasDamaged = false;
    [SerializeField] public bool forStopMove = false;
    public bool forStopRotate = false;

    public AttackDebugGizmo gizmo;

    public UnityEvent onInteraction;

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
        ani = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (stats.isDash || Equipment.MainWeapon == null) return;
        if (GameManager.instance.isStop) return;

        if (inputManager.isAttackPressed)
        {
            if (Equipment.MainWeapon.CanAttack())
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

    private IEnumerator superArmor()
    {
        hasDamaged = true;
        yield return new WaitForSeconds(stats.InvincibilityTime);
        hasDamaged = false;
    }

    public void takeDamage(int damage, Vector3 damagePos)
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
        transform.GetComponent<Rigidbody>().AddForce(-dir * stats.KnockBackForce, ForceMode.Impulse);
    }

    public void OnWeaponAcquire(WeaponID ID)
    {
        WeaponAbstract[] weapons = GetComponentsInChildren<WeaponAbstract>(true);

        WeaponAbstract target = null;

        foreach (WeaponAbstract weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
            if (weapon.weaponID == ID)
            {
                target = weapon;
                break;
            }
        }

        if (target == null)
        {
            return;
        }

        Equipment.EquipWeapon(target);
        Equipment.MainWeapon.Initialize(this.ani);

        ani.runtimeAnimatorController = target.overrideController;

        if (gizmo.mainWeapon == null)
        {
            gizmo.mainWeapon = target;//gizmo
        }
        else
        {
            gizmo.subWeapon = gizmo.mainWeapon;
            gizmo.mainWeapon = target;
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
