using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : EnemyStateAbstract
{
    [SerializeField] float attackSpeed = 10f;
    [SerializeField] float dashDuration = 0.5f;

    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    private IEnumerator Attack_Co(float attackSpeed)
    {
        state = EnemyState.attack;

        turnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        Vector3 dir = (targetPos - startPos).normalized;

        float timer = dashDuration;
        while (timer > 0f)
        {
            navMesh.Move(dir * attackSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        checkAttackTime();
        coroutine = null;

        turnOnNavmesh();

        state = EnemyState.chase;
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        coroutine = StartCoroutine(Attack_Co(attackSpeed));
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

