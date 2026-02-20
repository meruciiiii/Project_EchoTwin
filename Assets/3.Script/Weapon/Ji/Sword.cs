using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponAbstract
{
    private List<Collider> getTargetInSector()
    {
        List<Collider> Targets = new List<Collider>();

        GameObject player = stats.gameObject;
        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position;
        float range = weaponData.attackRange;

        Collider[] hits = Physics.OverlapSphere(centerPos, range);

        foreach(Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dirToTarget = (hit.transform.position - centerPos).normalized;

            if(Vector3.Angle(forward, dirToTarget)<attackAngle * 0.5f)
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

    //    Vector3 targetPos = new Vector3(player_XSize, 1f, weaponData.attackRange * 0.5f);

    //    Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

    //    lastAttackInfo = new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.red };//gizmo
    //    hasDebugInfo = true;//gizmo

    //    return hits;
    //}

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;

        AttackTimeChecker();

        SetAnimator();
        Debug.Log($"combo count = {comboCount}");

        List<Collider> targets = getTargetInSector();

        foreach(Collider target in targets)
        {
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

        foreach(Collider target in context.hitTargets)
        {
            GameObject player = stats.gameObject;

            Vector3 forward = player.transform.forward;
            Vector3 centerPos = target.transform.position;
            float range = weaponData.attackRange;

            Collider[] hits = Physics.OverlapSphere(centerPos, range);

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                Vector3 dirToTarget = (hit.transform.position - centerPos).normalized;

                if (Vector3.Angle(forward, dirToTarget) < attackAngle * 0.5f)
                {
                    hit.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage() * weaponData.echoDMGRatio);
                }
            }

            echoAttackInfos.Add(new AttackDebugInfo
            {
                shape = AttackShape.sector,
                center = centerPos,
                size = new Vector3(range, 0, 0),
                rotation = player.transform.rotation,
                color = Color.red,
                angle = attackAngle,
                direction = forward,
                ratio = 1f
            });
            hasDebugInfo = true;



            //Vector3 targetPos = new Vector3(player_XSize, 1f, weaponData.attackRange * 0.5f);

            //echoAttackInfos.Add(new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.cyan });//gizmo

            //Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

            //foreach(Collider hit in hits)
            //{
            //    if(hit.CompareTag("Enemy"))
            //    {
            //        hit.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage() * weaponData.echoDMGRatio);
            //    }
            //}
        }
        //attack 과 똑같이 가고 damage * data.damageratio 만큼
        //mainWeapon 에 닿은 target의 위치에 기본공격과 같은 크기의 범위만큼 공격
    }
}
