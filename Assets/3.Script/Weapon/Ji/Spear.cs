using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : WeaponAbstract
{
    [SerializeField] float echoLengthMultiple = 2f;

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

            Vector3 dirToTarget = (hit.transform.position - centerPos).normalized;

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

    private Collider[] getTargetInRange()
    {
        GameObject player = stats.gameObject;
        float player_XSize = player.GetComponent<CapsuleCollider>().radius;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);

        Vector3 targetPos = new Vector3(player_XSize * 0.25f, 1f, weaponData.attackRange * 0.5f);

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        lastAttackInfo = new AttackDebugInfo 
        { 
            shape = AttackShape.box,
            center = centerPos, 
            size = targetPos, 
            rotation = player.transform.rotation, 
            color = Color.red
        };//gizmo
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

        AttackTimeChecker();

        SetAnimator();
        Debug.Log($"combo count = {comboCount}");

        if (comboCount != 0)
        {
            Collider[] targets = getTargetInRange();

            foreach (Collider target in targets)
            {
                if (!target.CompareTag("Enemy")) continue;

                context.hitTargets.Add(target);
                target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());

                enemyKnockback(target);
            }
        }
        else
        {
            List<Collider> targets = getTargetInSector();

            foreach(Collider target in targets)
            {
                context.hitTargets.Add(target);
                target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());

                enemyKnockback(target);
            }
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

        Vector3 targetPos = new Vector3(player_XSize * 0.25f, 1f, weaponData.attackRange * echoLengthMultiple);

        echoAttackInfos.Add(new AttackDebugInfo
        {
            shape = AttackShape.box,
            center = centerPos,
            size = targetPos,
            rotation = player.transform.rotation,
            color = Color.cyan
        });

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            hit.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage() * weaponData.echoDMGRatio);
        }
        //hits 에게 데미지
    }
}
