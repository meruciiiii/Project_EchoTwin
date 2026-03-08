using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : WeaponAbstract
{
    [SerializeField] GameObject daggerPrefab;

    private List<Collider> getTargetInSector()
    {
        List<Collider> Targets = new List<Collider>();

        GameObject player = stats.gameObject;
        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position;
        float range = calcAttackRange(weaponData.attackRange);

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
    //    float player_XSize = player.GetComponent<CapsuleCollider>().radius;

    //    Vector3 forward = player.transform.forward;
    //    Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

    //    Vector3 targetPos = new Vector3(player_XSize * 0.5f, 1f, weaponData.attackRange * 0.5f);

    //    Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

    //    lastAttackInfo = new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.red };//gizmo
    //    hasDebugInfo = true;//gizmo

    //    return hits;
    //}

    //private float getDamage()
    //{
    //    float totalDamage = stats.PlayerDMG + calcDamage();

    //    return totalDamage;
    //}

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;

        AttackTimeChecker();

        SetAnimator();
        if (comboCount == weaponData.comboCount - 1)
        {
            SoundManager.SendEvent(SoundType.SFX_DaggerAttack2);
        }
        else if (comboCount == 0)
        {
            
        }
        else
        {
            SoundManager.SendEvent(SoundType.SFX_DaggerAttack1);
        }
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
        //mainWeapon 에 닿은 적들에게 칼이 날라가 데미지를 입힘

        Vector3 spawnPos = stats.gameObject.transform.position;//+ 뒤쪽 랜덤으로 -> 플레이어 근처 어딘가에 스폰

        foreach (Collider target in context.hitTargets)
        {
            GameObject dagger = Instantiate(daggerPrefab, spawnPos, Quaternion.identity);
            ThrowDagger throwDagger = dagger.GetComponent<ThrowDagger>();
            if (throwDagger == null)
            {
                Destroy(dagger);
                continue;
            }

            throwDagger.Init(target.transform, calcEchoDamage());
        }
    }
}
