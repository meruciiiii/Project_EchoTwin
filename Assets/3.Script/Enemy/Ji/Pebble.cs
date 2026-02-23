using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pebble : EnemyStateAbstract
{
    [SerializeField] private GameObject projectile;
    [SerializeField] float buffer = 0.5f;
    [SerializeField] float duration = 1f;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }
        if (state == EnemyState.dead) return;

        // 속도가 있으면 Run, 없으면 Idle
        if (ani != null)
        {
            ani.SetBool("Run", navMesh.velocity.magnitude > 0.1f);
        }

        if ((player.transform.position - transform.position).normalized.x > 0.01)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        Move();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (coroutine != null) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        if (ani != null) ani.SetTrigger("Attack");

        checkAttackTime();

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float timer = 0f;

        projectile.transform.position = startPos;
        projectile.SetActive(true);
        if(spriteRenderer.flipX)
        {
            projectile.GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
        if(!spriteRenderer.flipX)
        {
            projectile.GetComponentInChildren<SpriteRenderer>().flipX = false;
        }

        while (timer < duration)
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

                if(state != EnemyState.dead)
                {
                    state = EnemyState.chase;
                    TurnOnNavmesh();
                }
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

        //BodyAttack(standardRange);

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance > enemyData.attackRange + buffer)
        {
            setPlayerPos();
        }
        else if (distance < enemyData.attackRange - buffer)
        {
            Runaway();
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }

    private void Runaway()
    {
        Vector3 dir = transform.position - player.transform.position;
        Vector3 runPos = transform.position + dir.normalized * 2f;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(runPos, out hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
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
        if (state == EnemyState.dead) return;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            navMesh.isStopped = false;
            //navMesh.enabled = true;
            navMesh.Warp(hit.position);
        }
        else
        {
            state = EnemyState.dead;
        }
    }
}
