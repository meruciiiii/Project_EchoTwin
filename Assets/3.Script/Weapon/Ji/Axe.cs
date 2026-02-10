using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : WeaponAbstract
{
    [SerializeField] GameObject axePrefab;

    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(weaponData.attackRange * 0.5f, 1f, weaponData.attackRange * 0.5f);

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

        UpdateComboState();
        
        SetAnimator();

        Collider[] targets = getTargetInRange();

        foreach (Collider target in targets)
        {
            if (!target.CompareTag("Enemy")) continue;

            context.hitTargets.Add(target);
            target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());
        }
    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho(AttackContext context)
    {
        //mainWeapon 공격시 생성되어 플레이어 주변 공전. 닿을 시 데미지

        GameObject spawnAxe = Instantiate(axePrefab, stats.transform.position, Quaternion.identity);
        spawnAxe.GetComponent<OrbitAxe>().Init(stats.transform, calcDamage() * weaponData.echoDMGRatio);
    }
}
