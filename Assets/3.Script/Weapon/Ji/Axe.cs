using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : WeaponAbstract
{
    [SerializeField] GameObject axePrefab;

    private List<Collider> getTargetInSector()
    {
        List<Collider> Targets = new List<Collider>();

        GameObject player = stats.gameObject;
        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position;
        float range = weaponData.attackRange;

        Collider[] hits = Physics.OverlapSphere(centerPos, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dirToPivot = (hit.transform.position - centerPos).normalized;
            if (Vector3.Dot(forward, dirToPivot) < -0.2f) continue;

            Vector3 closePoint = hit.ClosestPoint(centerPos);
            closePoint.y = centerPos.y;

            Vector3 dirToTarget = (closePoint - centerPos).normalized;
            if (dirToTarget == Vector3.zero) dirToTarget = forward;

            if (Vector3.Angle(forward, dirToTarget) < attackAngle * 0.5f)
            {
                Targets.Add(hit);
            }
        }

        lastAttackInfo = new AttackDebugInfo
        {
            shape = AttackShape.sector,
            center = centerPos,
            size = new Vector3(range, 0, 0),
            rotation = player.transform.rotation,
            color = Color.red,
            angle = attackAngle,
            direction = forward,
            ratio = 1f
        };
        hasDebugInfo = true;

        return Targets;
    }

    //private Collider[] getTargetInRange()
    //{
    //    GameObject player = stats.gameObject;

    //    Vector3 forward = player.transform.forward;
    //    Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

    //    Vector3 targetPos = new Vector3(weaponData.attackRange * 0.5f, 1f, weaponData.attackRange * 0.5f);

    //    Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

    //    lastAttackInfo = new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.red };//gizmo
    //    hasDebugInfo = true;//gizmo

    //    return hits;
    //}

    private float getDamage()
    {
        float totalDamage = stats.PlayerDMG + calcDamage();

        return totalDamage;
    }

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;

        AttackTimeChecker();

        SetAnimator();
        Debug.Log($"combo count = {comboCount}");

        List<Collider> targets = getTargetInSector();

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
        //mainWeapon 공격시 생성되어 플레이어 주변 공전. 닿을 시 데미지

        GameObject spawnAxe = Instantiate(axePrefab, stats.transform.position, Quaternion.identity);
        spawnAxe.GetComponent<OrbitAxe>().Init(stats.transform, calcDamage() * weaponData.echoDMGRatio);
    }
}
