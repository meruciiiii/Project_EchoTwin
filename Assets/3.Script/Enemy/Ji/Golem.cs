using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyStateAbstract
{
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 0.5f;

    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] private float projectileDuration = 1f;

    [SerializeField] private GameObject projectile;

    protected override void Update()
    {
        base.Update();
        Attack();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (coroutine != null) return;
        if (!canAttack())
        {
            Move();
            return;
        }

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(targetPos, startPos);

        if (distance > enemyData.attackRange)
        {
            coroutine = StartCoroutine(ProjectileAttack_Co(targetPos, startPos));
        }
        else if(distance < enemyData.attackRange)
        {
            coroutine = StartCoroutine(DashAttack_Co(targetPos, startPos));
        }

        // 플레이어와의 거리가 멀다면 원거리 공격 가까우면 근거리 공격
    }

    private IEnumerator ProjectileAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;

        TurnOffNavmesh();
        SoundManager.SendEvent(SoundType.SFX_Golem2);
        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        if (ani != null) ani.SetTrigger("Attack");

        checkAttackTime();
        float shootHeight = 1.1f; 

        startPos.y += shootHeight;
        targetPos.y += shootHeight;

        Vector3 dir = (targetPos - startPos).normalized;

        float timer = 0f;

        projectile.transform.position = startPos;
        projectile.SetActive(true);

        while (timer < projectileDuration)
        {
            if (state == EnemyState.dead)
            {
                yield break;
            }

            if (!projectile.activeSelf)
            {
                projectile.transform.position = startPos;
                projectile.SetActive(false);

                coroutine = null;

                if (state != EnemyState.dead)
                {
                    state = EnemyState.chase;
                    TurnOnNavmesh();
                }
                yield break;
            }

            timer += Time.deltaTime;
            float t = timer / projectileDuration;
            projectile.transform.position += dir * projectileSpeed * Time.deltaTime;

            yield return null;
        }
        projectile.transform.position = startPos;
        projectile.SetActive(false);

        coroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    private IEnumerator DashAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;

        TurnOffNavmesh();
        if (ani != null) ani.SetTrigger("Attack 2");

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();


        Vector3 dir = (targetPos - startPos).normalized;
        dir.y = 0f;

        SoundManager.SendEvent(SoundType.SFX_Golem1);
        float timer = dashDuration;
        while (timer > 0f)
        {
            //navMesh.Move(dir * attackSpeed * Time.deltaTime);

            transform.position += dir * dashSpeed * Time.deltaTime;

            timer -= Time.deltaTime;
            yield return null;
        }

        coroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;
        if (coroutine != null) return;

        setPlayerPos();
    }

    protected override void TurnOffNavmesh()
    {
        navMesh.isStopped = true;
        navMesh.ResetPath();
        //navMesh.enabled = false;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
    }

    protected override void TurnOnNavmesh()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            navMesh.enabled = true;
            navMesh.Warp(hit.position);
        }
        else
        {
            StartCoroutine(ReturnToField_Co());
        }
    }

    private IEnumerator ReturnToField_Co()
    {
        state = EnemyState.knockback;
        float returnSpeed = enemyData.moveSpeed * 1.5f;

        if (navMesh.enabled && navMesh.isOnNavMesh)
        {
            navMesh.isStopped = true;
            navMesh.ResetPath();
        }
        navMesh.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        while (state != EnemyState.dead)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            dir.y = 0f;
            transform.position += dir * returnSpeed * Time.deltaTime;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                navMesh.enabled = true;
                navMesh.Warp(hit.position);
                navMesh.isStopped = false;
                state = EnemyState.chase;
                yield break;
            }
            yield return null;
        }
    }

    protected override bool isItOnTheGround()
    {
        return true;
    }
}
