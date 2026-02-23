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

        SetAnimator(); // 공격 애니메이션 트리거 (Trigger "Attack")

        // 애니메이션 상태가 바뀔 때까지 한 프레임 대기
        yield return null;

        // [핵심 로직] 버튼을 누르고 있는 동안 무한 루프
        while (input.isAttackPressed)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsTag("Attack"))
            {
                // 애니메이션이 0.5f(절반)에 도달하면 속도를 0으로 만들어 멈춤 (차징 대기)
                if (stateInfo.normalizedTime >= 0.3f)
                {
                    AniSpeed(0f);
                }
            }
            yield return null;
        }

        AnimatorStateInfo finalState = animator.GetCurrentAnimatorStateInfo(0);

        // 1. 0.5f 미만에서 뗐다면 공격 취소
        if (finalState.normalizedTime < 0.3f)
        {
            cancleCharging();
            yield break;
        }

        // 2. 0.5f 이상에서 뗐다면 공격 실행
        // 멈췄던 애니메이션 속도를 다시 1로 복구
        AniSpeed(1f);

        // 버튼을 뗀 후부터 추가 차징 시간 측정 (기존 로직 유지)
        time = 0f;
        
        /*
        while (input.isAttackPressed) // 이미 위에서 뗐으므로 이 루프는 스킵될 것임
        {
            time += Time.deltaTime;
            yield return null;
        }
        */

        // 공격 타격 시점까지 대기 (애니메이션의 남은 부분 재생 시간)
        yield return new WaitForSeconds(0.2f / weaponData.attackSpeed);

        // 타격 판정
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
