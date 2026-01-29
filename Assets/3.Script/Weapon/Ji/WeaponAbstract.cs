using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected CharacterData characterData;

    protected float lastAttackTime;
    protected int resonanceCount;

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

    protected float calcDamage()
    {
        return weaponData.baseDamage + characterData.valuePerLv;
    }

    public abstract void Attack(AttackContext context);

    public abstract void ChargingAttack();

    public abstract void OnEcho(AttackContext context);
}
