using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEye : EnemyStateAbstract
{
    [SerializeField] private float duration = 0.5f;

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Move();
    }
    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        coroutine = StartCoroutine(Attack_Co(targetPos, startPos));
    }

    private IEnumerator Attack_Co(Vector3 destPos, Vector3 startPos)
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        float timer = 0f;
        bool isAttacked = false;

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        checkAttackTime();

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.position = Vector3.Lerp(startPos, destPos, t);
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

        coroutine = null;

        TurnOnNavmesh();
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        BodyAttack(enemyData.attackRange);

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
        {
            state = EnemyState.chase;
            setPlayerPos();
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }

    protected override void TurnOnNavmesh()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
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
        float returnSpeed = enemyData.moveSpeed * 1.5f;

        while (true)
        {
            Vector3 dir = (player.transform.position = transform.position).normalized;
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
