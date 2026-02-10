using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyStateAbstract
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private int bodyAttackMultiple = 3;
    [SerializeField] private int rangeMultiple = 2;
    [SerializeField] private GameObject projectile;

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Attack();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (!canAttack()) Move();

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(targetPos, startPos);

        if (distance > enemyData.attackRange * rangeMultiple)
        {
            Move();
        }
        else if (distance < enemyData.attackRange * rangeMultiple && distance > enemyData.attackRange)
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(ProjectileAttack_Co(targetPos, startPos));
        }
        else
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(DashAttack_Co(targetPos, startPos));
        }

        // 플레이어와의 거리가 멀다면 원거리 공격 가까우면 근거리 공격
    }

    private IEnumerator ProjectileAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;
        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();

        float timer = 0f;
        float duration = 1f;

        projectile.transform.position = startPos;
        projectile.SetActive(true);

        while (timer < duration)
        {
            if (state == EnemyState.dead || !projectile.activeSelf)
            {
                yield break;
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            projectile.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }
        projectile.transform.position = startPos;
        projectile.SetActive(false);

        coroutine = null;
        TurnOnNavmesh();
    }

    private IEnumerator DashAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;
        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);

        bool isAttacked = false;
        Vector3 dir = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        while (distance > 0f)
        {
            //navMesh.Move(dir * enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime);
            Vector3 destPos = transform.position + (dir * enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime);
            transform.position = destPos;

            distance -= enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime;

            if (!isAttacked)
            {
                if (BodyAttack(enemyData.attackRange))
                {
                    isAttacked = true;
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);//애니메이션을 위한 여유시간
        transform.position = startPos;

        checkAttackTime();

        coroutine = null;
        TurnOnNavmesh();
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        BodyAttack(standardRange);

        setPlayerPos();
    }

    protected override void TurnOnNavmesh()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            navMesh.enabled = true;
            navMesh.Warp(hit.position);
            state = EnemyState.chase;
        }
        else
        {
            StartCoroutine(ReturnToField_Co());
        }
    }

    private IEnumerator ReturnToField_Co()
    {
        float returnSpeed = enemyData.moveSpeed * 1.5f;

        while (true)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            transform.position += dir * returnSpeed * Time.deltaTime;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navMesh.enabled = true;
                navMesh.Warp(hit.position);
                state = EnemyState.chase;
                yield break;
            }
            yield return null;
        }
    }
}
