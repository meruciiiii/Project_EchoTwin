using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bringer : EnemyStateAbstract
{
    [SerializeField] private GameObject projectile;

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

        if (navMesh.enabled && navMesh.desiredVelocity.sqrMagnitude > 0.01f)
        {
            lookDir = navMesh.desiredVelocity.normalized;
            lookDir.y = 0f;

            //GetComponentInChildren<SpriteRenderer>().flipX = (lookDir.x < -0.1f);

            if(lookDir.x > 0.01f)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            if(lookDir.x < 0.01f)
            {
                transform.rotation = Quaternion.identity;
            }

        }

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

        if (distance > enemyData.attackRange - 0.5f)
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(ProjectileATK(targetPos));
        }
        else
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(Attack_Co());
        }
    }

    private IEnumerator ProjectileATK(Vector3 targetPos)
    {
        state = EnemyState.attack;
        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);

        if (ani != null) ani.SetTrigger("Attack 2");

        checkAttackTime();

        projectile.transform.position = targetPos;
        projectile.SetActive(true);
        //bullet animator
        yield return new WaitForSeconds(1.6f);
        projectile.SetActive(false);

        coroutine = null;
        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);

        if (ani != null) ani.SetTrigger("Attack");

        checkAttackTime();

        AreaAttack(enemyData.attackRange, 270f);

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
}
