using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : EnemyStateAbstract
{
    protected override void Update()
    {
        base.Update();
        Move();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        //animator
        if (ani != null) ani.SetTrigger("Attack");

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);

        checkAttackTime();

        AreaAttack(enemyData.attackRange + radius, 180f);

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
        float buffer = 0.2f;

        if (distance > enemyData.attackRange - buffer)
        {
            setPlayerPos();
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }
}
