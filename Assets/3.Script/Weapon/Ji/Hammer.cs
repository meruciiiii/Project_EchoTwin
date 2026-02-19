using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : WeaponAbstract
{
    private float time = 0f;
    private Coroutine coroutine;

    private List<Collider> getTargetInSector()
    {
        List<Collider> Targets = new List<Collider>();

        GameObject player = stats.gameObject;
        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);
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
            shape = AttackShape.sphere,
            center = centerPos,
            size = Vector3.one * range,
            rotation = player.transform.rotation,
            color = Color.red,
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

    public override void Attack(AttackContext context)
    {
        if (!CanAttack()) return;
        if (isCharging) return;
        coroutine = StartCoroutine(Attack_Co(context));
    }

    private IEnumerator Attack_Co(AttackContext context)
    {
        isCharging = true;
        action.GetComponent<Rigidbody>().isKinematic = true;

        SetAnimator();//무기 든 모션

        time = 0f;
        yield return new WaitForSeconds(0.1f);
        AniSpeed(0f);

        while (time < 0.3f)
        {
            if (!input.isAttackPressed)
            {
                cancleCharging();
                yield break;
            }
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        while (input.isAttackPressed)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = Mathf.Min(time, 3f);

        AniSpeed(1f);

        yield return new WaitForSeconds(0.2f / weaponData.attackSpeed);

        List<Collider> targets = getTargetInSector();

        foreach (Collider target in targets)
        {
            context.hitTargets.Add(target);
            target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());

            enemyKnockback(target);
        }

        action.GetComponent<Rigidbody>().isKinematic = false;
        coroutine = null;
        isCharging = false;
        isCancelled = false;
    }

    private void cancleCharging()
    {
        if (!isCharging) return;

        if (coroutine != null)
        {
            coroutine = null;
        }
        isCharging = false;
        AniSpeed(1f);
        animator.Play("Move", 0, 0);
        stats.GetComponent<PlayerAction>().forStopMove = false;
        isCancelled = true;
    }

    private void AniSpeed(float holdSpeed = 1f)
    {
        float finalSpeed = weaponData.attackSpeed * holdSpeed;
        animator.SetFloat("AttackSpeed", finalSpeed);
    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho(AttackContext context)
    {
        echoAttackInfos.Clear();//gizmo

        //mainWeapon 공격시 기본공격과 같은 범위와 위치에 추가 데미지

        GameObject player = stats.gameObject;

        Vector3 forward = player.transform.forward;
        Vector3 centerPos = player.transform.position + forward * (weaponData.attackRange * 0.5f);
        float range = weaponData.attackRange;

        echoAttackInfos.Add(new AttackDebugInfo
        {
            shape = AttackShape.sphere,
            center = centerPos,
            size = new Vector3(range, 0, 0),
            rotation = player.transform.rotation,
            color = Color.cyan,
            ratio = 1f
        });

        Collider[] hits = Physics.OverlapSphere(centerPos, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            hit.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage());
            StartCoroutine(EnemyGatherng(centerPos, hit));
        }
    }

    private IEnumerator EnemyGatherng(Vector3 centerPos, Collider target)
    {
        Vector3 targetPos = target.transform.position;

        float time = 0f;
        float duration = 1f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            target.transform.position = Vector3.Lerp(targetPos, centerPos, t);
            yield return null;
        }
        target.transform.position = centerPos;
    }

    protected override float calcDamage()
    {
        return weaponData.baseDamage * time + stats.PlayerDMG;// + characterData.valuePerLv 이 부분 정리
    }
}
