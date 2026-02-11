using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : WeaponAbstract
{
    [SerializeField] GameObject daggerPrefab;

    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(player_XSize * 0.5f, 1f, weaponData.attackRange * 0.5f);

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

        checkAttackTime();

        SetAnimator();
        UpdateComboState();


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
        //mainWeapon 에 닿은 적들에게 칼이 날라가 데미지를 입힘

        Vector3 spawnPos = stats.gameObject.transform.position;//+ 뒤쪽 랜덤으로 -> 플레이어 근처 어딘가에 스폰

        foreach (Collider target in context.hitTargets)
        {
            GameObject dagger = Instantiate(daggerPrefab, spawnPos, Quaternion.identity);
            dagger.GetComponent<ThrowDagger>().Init(target.transform, calcDamage() * weaponData.echoDMGRatio);
        }
    }
}
