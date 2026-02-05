using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : EnemyStateAbstract
{
    [SerializeField] private int shieldCount = 10;
    [SerializeField] private float reduceRatio = 0.9f;

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;
        if (shieldCount > 0)
        {
            damage *= 1 - reduceRatio;
            effect.Flash(1, 0.5f);
        }

        currentHP -= damage;
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

        turnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator

        Collider[] hits = Physics.OverlapSphere(transform.position, enemyData.attackRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player.takeDamage(enemyData.damage,transform.position);
            }
        }
        checkAttackTime();
        coroutine = null;

        turnOnNavmesh();

        state = EnemyState.chase;
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
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }
}
