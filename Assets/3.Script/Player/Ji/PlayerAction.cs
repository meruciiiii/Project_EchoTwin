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
    private Rigidbody rb;
    private Animator ani;

    public bool isKnockback = false;
    public bool hasDamaged = false;
    public bool forStopMove = false;
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
        TryGetComponent(out rb);
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

    public void checkWeapon()
    {
        if(Equipment.MainWeapon == null)
        {
            GameManager.instance.isGetWeapon = false;
        }
        else
        {
            GameManager.instance.isGetWeapon = true;
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

    public void takeDamage(int damage, Vector3 damagePos, float knockbackForce)
    {
        if (hasDamaged) return;
        if (stats.isDash) return;
        if (ani != null) ani.SetTrigger("TakeDamage");
        SoundManager.SendEvent(SoundType.SFX_PlayerHit);

        stats.takeDamage(damage);

        Vector3 dir = (damagePos - transform.position).normalized;
        knockback(dir, knockbackForce);

        StartCoroutine(superArmor());

        effect.Flash(stats.FlashAmount, stats.FlashDuration);

        if (stats.isDead)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Die);
        }
    }

    private void knockback(Vector3 dir, float knockbackForce)
    {
        if (Equipment.MainWeapon.IsCharging) return;

        if (isKnockback) return;

        StartCoroutine(knockBack_Co(dir, knockbackForce));
    }

    private IEnumerator knockBack_Co(Vector3 dir, float knockbackForce)
    {
        isKnockback = true;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(-dir * stats.KnockBackForce * knockbackForce, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();

        while (rb.linearVelocity.magnitude > stats.KnockBackForce * 0.5f) yield return null;

        isKnockback = false;
        //transform.GetComponent<Rigidbody>().AddForce(-dir * stats.KnockBackForce, ForceMode.Impulse);
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
        checkWeapon();

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
