using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponAbstract
{
    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(player_XSize, 1f, weaponData.attackRange * 0.5f);

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

        foreach(Collider target in targets)
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
        foreach(Collider target in context.hitTargets)
        {
            GameObject player = stats.gameObject;
            float player_XSize = player.GetComponent<CapsuleCollider>().radius;

            Vector3 forward = player.transform.forward;
            Vector3 centerPos = target.transform.position + forward * (weaponData.attackRange * 0.5f);

            Vector3 targetPos = new Vector3(player_XSize, 1f, weaponData.attackRange * 0.5f);

            Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

            //hits 에게 데미지 주기
        }
        //attack 과 똑같이 가고 damage * data.damageratio 만큼
        //mainWeapon 에 닿은 target의 위치에 기본공격과 같은 크기의 범위만큼 공격
    }
}
