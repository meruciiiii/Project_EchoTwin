using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyStateAbstract
{
    [SerializeField] private float shieldDgree = 0.3f;
    [SerializeField] private float reduceRatio = 0.9f;

    protected override void Awake()
    {
        base.Awake();
        navMesh.updateRotation = true;
    }

    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;

        Vector3 attackerPos = player.transform.position;
        Vector3 dir = (attackerPos = transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);

        if (dot > shieldDgree)
        {
            damage *= 1 - reduceRatio;
            effect.Flash(1, 0.5f);//¸·¾ÒÀ» ½Ã ¹øÂ½ ÀÌÆåÆ®
        }

        currentHP -= damage;
        checkOnDie();
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

    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        state = EnemyState.chase;

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
