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

    [SerializeField] private GameObject dieUI;

    //private Coroutine forSubscribe_Co;
    //private bool isSubscribed = false;

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

    private void OnEnable()
    {
        if (GameManager.instance == null) return;
        GameManager.instance.playerDie += onDie;

        //if (forSubscribe_Co != null)
        //{
        //    StopCoroutine(forSubscribe_Co);
        //    forSubscribe_Co = null;
        //}
        //forSubscribe_Co = StartCoroutine(subscribe_Co());
    }

    private void OnDisable()
    {
        if (GameManager.instance == null) return;
        GameManager.instance.playerDie -= onDie;

        //if (forSubscribe_Co != null)
        //{
        //    StopCoroutine(forSubscribe_Co);
        //    forSubscribe_Co = null;
        //}

        //if (isSubscribed && GameManager.instance != null)
        //{
        //    GameManager.instance.playerDie -= onDie;
        //}
        //isSubscribed = false;
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

    //private IEnumerator subscribe_Co()
    //{
    //    while (GameManager.instance == null)
    //    {
    //        yield return null;
    //    }

    //    if (!isSubscribed)
    //    {
    //        GameManager.instance.playerDie += onDie;
    //        isSubscribed = true;
    //    }

    //    forSubscribe_Co = null;
    //}

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

    public bool TryBuyShopItem(shopItem itemType, int price, int value)
    {
        if (stats == null) return false;
        if (stats.Gold < price) return false;

        switch (itemType)
        {
            case shopItem.Heart:
                if (stats.CurrentHP >= stats.MaxHP) return false;
                break;

            case shopItem.Cristal:
                break;

            case shopItem.refillEcho:
                if (Equipment == null) return false;
                if (Equipment.SubWeapon == null) return false;
                if (Equipment.SubWeapon.resonanceCount >= Equipment.SubWeapon.weaponData.resonanceCount) return false;
                break;
        }

        if (!stats.TryUseGold(price)) return false;

        switch (itemType)
        {
            case shopItem.Heart:
                for (int i = 0; i < value; i++)
                {
                    stats.getHeart(value);
                }
                break;

            case shopItem.Cristal:
                stats.getCristal(value);
                break;

            case shopItem.refillEcho:
                int maxCount = Equipment.SubWeapon.weaponData.resonanceCount;
                int nextCount = Equipment.SubWeapon.resonanceCount + value;

                if (nextCount > maxCount)
                {
                    nextCount = maxCount;
                }

                Equipment.SubWeapon.SetResonance(nextCount);

                if (GameManager.instance != null)
                {
                    GameManager.instance.setResonance();
                }
                break;
        }

        return true;
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

    private void onDie()
    {
        ani.SetTrigger("Die");
        dieUI.SetActive(true);
        Equipment.SubWeapon = null;
        Equipment.MainWeapon = null;
        stats.resetGold();
    }

    private void knockback(Vector3 dir, float knockbackForce)
    {
        //if (Equipment.MainWeapon.IsCharging) return;

        if (isKnockback) return;
        isKnockback = true; 
        StartCoroutine(knockBack_Co(dir, knockbackForce));
    }

    private IEnumerator knockBack_Co(Vector3 dir, float knockbackForce)
    {
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 finalDir = dir;
        finalDir.y = 0;

        rb.AddForce(finalDir * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f); // �˹� �ð� (�ʿ�� ������ ��ü)

        isKnockback = false;
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

        if (GameManager.instance != null)
        {
            GameManager.instance.TurnWeaponUI(Equipment.MainWeapon, Equipment.SubWeapon);
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
