using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : EnemyStateAbstract
{
    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void Attack()
    {
        if (!canAttack()) return;

        state = EnemyState.attack;
        checkAttackTime();

        effect.ChargeEffect(enemyData.attackSpeed);
        WaitForSeconds wfs = new WaitForSeconds(enemyData.attackSpeed);
        //animator
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyData.attackRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player.takeDamage(enemyData.damage);
            }
        }
        state = EnemyState.chase;
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;

        state = EnemyState.chase;

        float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
        if (sqrDist <= enemyData.attackRange * enemyData.attackRange)
        {
            turnOffNavmesh();
            Attack();
        }
        else
        {
            turnOnNavmesh();
            setPlayerPos();
        }
    }
}
