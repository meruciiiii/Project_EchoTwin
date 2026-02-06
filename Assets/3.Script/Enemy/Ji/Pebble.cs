using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : EnemyStateAbstract
{
    [SerializeField] GameObject projectile;

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

    private IEnumerator Attack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;

        turnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        checkAttackTime();

        float timer = 0f;
        float duration = 1f;

        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        while (timer < duration)
        {
            if (state == EnemyState.dead)
            {
                projectile.SetActive(false);
                yield break;
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            projectile.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }
        projectile.transform.position = transform.position;
        projectile.SetActive(false);

        coroutine = null;

        turnOnNavmesh();

        state = EnemyState.chase;
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        BodyAttack(standardRange);

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
        if(UnityEngine.AI.NavMesh.SamplePosition(runPos, out hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }
}
