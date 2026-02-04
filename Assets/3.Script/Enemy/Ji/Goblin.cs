using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : EnemyStateAbstract
{
    private Coroutine coroutine;
    [SerializeField] float attackSpeed = 10f;
    [SerializeField] float dashDuration = 0.5f;

    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    private IEnumerator attack_Co(float attackTime)
    {
        state = EnemyState.attack;

        turnOffNavmesh();

        attackMotion(enemyData.attackSpeed);

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        Vector3 dir = (targetPos - startPos).normalized;

        float timer = dashDuration;
        while (timer > 0f)
        {
            navMesh.Move(dir * attackTime * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        checkAttackTime();

        state = EnemyState.chase;
        coroutine = null;

        turnOnNavmesh();
    }

    public override void Attack()
    {

    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
        {
            state = EnemyState.chase;
            setPlayerPos();
        }
        else if (distance < enemyData.attackRange - buffer)
        {
            state = EnemyState.chase;
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
            transform.LookAt(player.transform);
        }
    }
}

