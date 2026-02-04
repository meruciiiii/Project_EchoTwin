using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected CharacterData characterData;
    [SerializeField] protected PlayerStats stats;

    protected float lastAttackTime;
    protected int resonanceCount;
    protected int comboCount;

    public void SetComboCount()
    {
        comboCount = weaponData.comboCount;
    }

    public bool canCombo()
    {
        return comboCount > 0;
    }

    public void ConsumeComboCount()
    {
        comboCount--;
    }

    public  void SetResonance(int count)
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

    protected bool canAttack()
    {
        return Time.time >= lastAttackTime + (1f / weaponData.attackSpeed);
    }

    protected void checkAttackTime()
    {
        lastAttackTime = Time.time;
    }

    protected virtual void enemyKnockback(Collider target)
    {
        if(target.TryGetComponent<Iknockback>(out Iknockback kb))
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            kb.applyKnockback(dir, weaponData.knockback);
        }
    }

    protected float calcDamage()
    {
        return weaponData.baseDamage + stats.PlayerDMG;// + characterData.valuePerLv 이 부분 정리
    }

    public abstract void Attack(AttackContext context);

    public abstract void ChargingAttack();

    public abstract void OnEcho(AttackContext context);
}
