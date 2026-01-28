using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : WeaponAbstract
{
    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponentInParent<PlayerStats>();
    }

    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(weaponData.attackRange * 0.5f, 1f, weaponData.attackRange * 0.5f);

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        return hits;
    }

    private float getDamage()
    {
        float totalDamage = stats.PlayerDMG + calcDamage();

        return totalDamage;
    }

    public override void Attack()
    {
        if (!canAttack()) return;

        checkAttackTime();
    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho()
    {

    }
}
