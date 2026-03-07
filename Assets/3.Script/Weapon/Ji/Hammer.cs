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
        float range = calcAttackRange(weaponData.attackRange);
        Vector3 centerPos = player.transform.position + forward * (range * 0.5f);

        Collider[] hits = Physics.OverlapSphere(centerPos, range);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Targets.Add(hit);
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
        if (stats.GetComponent<PlayerAction>().isKnockback) return;
        coroutine = StartCoroutine(Attack_Co(context));
    }
    private IEnumerator Attack_Co(AttackContext context)
    {
        isCharging = true;
        isCancelled = false; 
        PlayerAction playerAction = stats.GetComponent<PlayerAction>(); // 참조 최적화
        action.GetComponent<Rigidbody>().isKinematic = true;

        SetAnimator(); // 공격 애니메이션 트리거 (Trigger "Attack")
        SoundManager.SendEvent(SoundType.SFX_HammerAttack1);

        yield return null;

        time = 0f;
        while (input.isAttackPressed)
        {
            if (playerAction.isKnockback)
            {
                cancleCharging();
                yield break;
            }

            time += Time.deltaTime; // 버튼 누르는 동안 시간 누적

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("Attack"))
            {
                if (stateInfo.normalizedTime >= 0.3f)
                {
                    AniSpeed(0f);
                }
            }
            yield return null;
        }
        if (playerAction.isKnockback || isCancelled)
        {
            if (isCharging) cancleCharging(); // 아직 처리 안 됐다면 처리
            yield break;
        }
        AnimatorStateInfo finalState = animator.GetCurrentAnimatorStateInfo(0);

        if (finalState.normalizedTime < 0.3f)
        {
            cancleCharging();
            yield break;
        }

        AniSpeed(1f);

        SoundManager.SendEvent(SoundType.SFX_HammerAttack2);

        yield return new WaitForSeconds(0.2f / calcAttackSpeed());

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
            StopCoroutine(coroutine); 
            coroutine = null;
        }
        isCharging = false;
        AniSpeed(1f);
        animator.Play("Move", 0, 0);
        stats.GetComponent<PlayerAction>().forStopMove = false;
        isCancelled = true;
        if (action != null)
        {
            action.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void AniSpeed(float holdSpeed = 1f)
    {
        float finalSpeed = calcAttackSpeed() * holdSpeed;
        animator.SetFloat("AttackSpeed", finalSpeed);
    }

    public override void ChargingAttack()
    {

    }

    public override void OnEcho(AttackContext context)
    {
        echoAttackInfos.Clear();
        GameObject player = stats.gameObject;
        if (context.hitTargets.Count > 0 && context.hitTargets[0] != null)
        {
            Vector3 dirToTarget = (context.hitTargets[0].transform.position - player.transform.position).normalized;
            dirToTarget.y = 0; // 수평 유지

            SoundManager.SendEvent(SoundType.SFX_HammerAttack1);

            float range = calcAttackRange(weaponData.attackRange);
            Vector3 centerPos = player.transform.position + dirToTarget * (range * 0.5f);

            if (attackEffects.Length > 0 && attackEffects[0].prefab != null)
            {
                AttackEffectData data = attackEffects[1];
                GameObject effect = Instantiate(data.prefab, centerPos, player.transform.rotation);

                effect.transform.localScale = Vector3.one * (data.scale * 1.0f);
            }

            Collider[] hits = Physics.OverlapSphere(centerPos, range);

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                stats.StartCoroutine(EnemyGatherng(centerPos, hit));
                hit.GetComponent<EnemyStateAbstract>().takeDamage(calcEchoDamage());
            }

            echoAttackInfos.Add(new AttackDebugInfo
            {
                shape = AttackShape.sphere,
                center = centerPos,
                size = Vector3.one * range,
                rotation = player.transform.rotation,
                color = Color.cyan,
                ratio = 1f
            });
            hasDebugInfo = true; // 기즈모 활성화
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
        float chargeRatio = Mathf.Clamp01(time / 2.0f);

        float damageMultiplier = Mathf.Lerp(0.2f, 1.0f, chargeRatio);

        return (weaponData.baseDamage * damageMultiplier) + stats.PlayerDMG;
    }
}
