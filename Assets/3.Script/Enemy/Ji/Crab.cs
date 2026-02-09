using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : EnemyStateAbstract
{
    [SerializeField] private int shieldCount = 10;
    [SerializeField] private float reduceRatio = 0.9f;

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Move();
    }

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;
        if (shieldCount > 0)
        {
            damage *= 1 - reduceRatio;
            effect.Flash(1, 0.5f);
        }

        currentHP -= damage;
        shieldCount--;
        checkOnDie();
    }

    public override void Attack()
    {
        if (!canAttack()) return;
        if (state == EnemyState.attack) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        checkAttackTime();

        AreaAttack(enemyData.attackRange, 180f);

        coroutine = null;

        TurnOnNavmesh();
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
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }
}
