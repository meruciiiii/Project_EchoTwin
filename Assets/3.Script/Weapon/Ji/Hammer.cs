using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : WeaponAbstract
{
    private bool isCharging = false;
    private float time = 0f;
    [SerializeField] float windUpTime = 0.3f;

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
        if (isCharging) return;
        StartCoroutine(Attack_Co(context));
    }

    private IEnumerator Attack_Co(AttackContext context)
    {
        isCharging = true;
        stats.GetComponent<PlayerAction>().isAttack = true;

        SetAnimator();//무기 든 모션
        yield return new WaitForSeconds(windUpTime);
        time = 0f;
        AniSpeed(0f);
        while (time < 0.5f)
        {
            if(!input.isAttackPressed)
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
        //checkAttackTime();
        //UpdateComboState();
        yield return new WaitForSeconds(0.2f / weaponData.attackSpeed);

        Collider[] targets = getTargetInRange();

        foreach (Collider target in targets)
        {
            if (!target.CompareTag("Enemy")) continue;

            context.hitTargets.Add(target);
            target.GetComponent<EnemyStateAbstract>().takeDamage(calcDamage() * time);

            enemyKnockback(target);
        }

        isCharging = false;
        stats.GetComponent<PlayerAction>().isAttack = false;
    }

    private void cancleCharging()
    {
        if (!isCharging) return;

        StopAllCoroutines();
        isCharging = false;
        AniSpeed(1f);
        animator.Play("Move",0,0);
        stats.GetComponent<PlayerAction>().isAttack = false;
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

        Vector3 targetPos = new Vector3(weaponData.attackRange * 0.5f, 1f, weaponData.attackRange * 0.5f);

        echoAttackInfos.Add(new AttackDebugInfo { center = centerPos, halfExtents = targetPos, rotation = player.transform.rotation, color = Color.cyan });//gizmo

        Collider[] hits = Physics.OverlapBox(centerPos, targetPos, player.transform.rotation);

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
