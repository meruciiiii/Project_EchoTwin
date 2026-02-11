using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : WeaponAbstract
{
    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(player_XSize*0.25f, 1f, weaponData.attackRange * 0.5f);

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        lastAttackInfo = new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.red };//gizmo
        hasDebugInfo = true;//gizmo

        return hits;
    }

    private float getDamage()
    {
        float totalDamage = stats.PlayerDMG + calcDamage();

        return totalDamage;
    }

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;
        Debug.Log($"combo count = {comboCount}");

        //checkAttackTime();

        //UpdateComboState();

        SetAnimator();

        Collider[] targets = getTargetInRange();

        foreach (Collider target in targets)
        {
            if (!target.CompareTag("Enemy")) continue;

            context.hitTargets.Add(target);
            target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());

            enemyKnockback(target);
        }
    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho(AttackContext context)
    {
        echoAttackInfos.Clear();//gizmo

        //mainWeapon 공격시 기본공격과 같은 위치에 공격. 다만 사거리는 조금 더 길 예정

        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange);

        Vector3 targetPos = new Vector3(player_XSize * 0.25f, 1f, weaponData.attackRange);

        echoAttackInfos.Add(new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.cyan });//gizmo

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        foreach(Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            hit.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage() * weaponData.echoDMGRatio);
        }
        //hits 에게 데미지
    }
}
