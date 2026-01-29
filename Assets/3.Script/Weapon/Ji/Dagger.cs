using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : WeaponAbstract
{
    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponentInParent<PlayerStats>();
    }

    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(player_XSize * 0.5f, 1f, weaponData.attackRange * 0.5f);

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        return hits;
    }

    private float getDamage()
    {
        float totalDamage = stats.PlayerDMG + calcDamage();

        return totalDamage;
    }

    public override void Attack(AttackContext context)
    {
        if (!canAttack()) return;

        checkAttackTime();

        Collider[] targets = getTargetInRange();

        foreach (Collider target in targets)
        {
            if (!target.CompareTag("Enemy")) continue;

            context.hitTargets.Add(target);
            //target stat 에 getdamage만큼 데미지
        }

    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho(AttackContext context)
    {
        //mainWeapon 에 닿은 적들에게 칼이 날라가 데미지를 입힘
        foreach (Collider target in context.hitTargets)
        {
            
            //target 에게 단검 날라가 데미지 주기
        }
    }

    private void spawnDagger()
    {

    }
}
