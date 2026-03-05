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

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;

        AttackTimeChecker();

        SetAnimator();
        if (comboCount == 0)
        {
            SoundManager.SendEvent(SoundType.SFX_SwordAttack2);
        }
        else
        {
            SoundManager.SendEvent(SoundType.SFX_SwordAttack1);
        }

        Debug.Log($"combo count = {comboCount}");

        List<Collider> targets = getTargetInSector();

        foreach (Collider target in targets)
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

        foreach (Collider target in context.hitTargets)
        {
            GameObject player = stats.gameObject;

            Vector3 forward = player.transform.forward;
            Vector3 centerPos = target.transform.position;
            float range = weaponData.attackRange;

            if (attackEffects.Length > 0 && attackEffects[0].prefab != null)
            {
                AttackEffectData data = attackEffects[0];

                GameObject effect = Instantiate(data.prefab, centerPos, player.transform.rotation);

                float scaleMultiplier = 1.2f; 
                effect.transform.localScale = Vector3.one * (data.scale * scaleMultiplier);
            }
            SoundManager.SendEvent(SoundType.SFX_SwordAttack1);

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
        }
    }
}