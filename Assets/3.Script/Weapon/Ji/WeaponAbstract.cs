using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    onehand,
    twohand,
    dual,
}

public abstract class WeaponAbstract : MonoBehaviour
{
    [SerializeField] public WeaponData weaponData;
    [SerializeField] protected CharacterData characterData;
    [SerializeField] protected PlayerStats stats;
    [SerializeField] protected Animator animator;
    [SerializeField] protected PlayerEquipment equipment;
    [SerializeField] protected InputManager input;

    public WeaponType weaponType;
    public GameObject DualWeapon;

    protected int resonanceCount;

    protected float lastAttackTime;
    protected int comboCount;
    protected bool isComboCooltime = false;

    protected float comboExpireTime;

    protected AttackDebugInfo lastAttackInfo;
    protected bool hasDebugInfo;
    protected List<AttackDebugInfo> echoAttackInfos = new List<AttackDebugInfo>();

    public AttackDebugInfo DebugInfo => lastAttackInfo;
    public bool HasDebugInfo => hasDebugInfo;
    public IReadOnlyList<AttackDebugInfo> EchoAttackInfos => echoAttackInfos;

    private void Awake()
    {
        comboCount = 0;
        SetResonance(10);
    }

    public void Initialize(Animator  playerAni)
    {
        this.animator = playerAni;
        animator.SetInteger("WeaponType", weaponData.ID);
    }

    #region Combo관련
    public bool CanAttack()
    {
        if (isComboCooltime) return false;

        if (Time.time < lastAttackTime) return false;

        return true;
    }

    protected void checkAttackTime()
    {
        float comboExpireTime = lastAttackTime + weaponData.attackSpeed;

        if(Time.time > comboExpireTime)
        {
            comboCount = 0;
        }

        lastAttackTime = Time.time + weaponData.attackSpeed;
    }

    protected void UpdateComboState()
    {
        comboCount++;

        if (comboCount >= weaponData.comboCount)
        {
            comboCount = 0;
            StartCoroutine(ComboCooltime_Co());
        }
    }

    protected IEnumerator ComboCooltime_Co()
    {
        isComboCooltime = true;
        yield return new WaitForSeconds(weaponData.comboCooltime + weaponData.attackSpeed);
        isComboCooltime = false;
    }

    protected void SetAnimator()
    {
        animator.SetInteger("ComboState", comboCount);
        animator.SetTrigger("Attack");
    }
    #endregion

    #region Echo관련
    public void SetResonance(int count)
    {
        resonanceCount = count;
    }

    public bool canEcho()
    {
        return resonanceCount > 0;
    }

    public void ConsumeResonance()
    {
        resonanceCount--;
    }
    #endregion

    protected virtual void enemyKnockback(Collider target)
    {
        if(target.TryGetComponent<Iknockback>(out Iknockback kb))
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            kb.applyKnockback(dir, weaponData.knockback);
        }
    }

    protected virtual float calcDamage()
    {
        return weaponData.baseDamage + stats.PlayerDMG;// + characterData.valuePerLv 이 부분 정리
    }

    public abstract void Attack(AttackContext context);

    public abstract void ChargingAttack();

    public abstract void OnEcho(AttackContext context);
}
