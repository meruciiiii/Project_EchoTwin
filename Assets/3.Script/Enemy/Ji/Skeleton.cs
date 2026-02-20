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

    protected override void Update()
    {
        base.Update();
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
            //막는 animator
            if (ani != null) ani.SetTrigger("Attack 2");
            effect.Flash(1, 0.5f);//막았을 시 번쩍 이펙트
        }

        currentHP -= damage;
        checkOnDie();
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

        AreaAttack(enemyData.attackRange, 180f);

        coroutine = null;

        TurnOnNavmesh();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (coroutine != null) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        //BodyAttack(standardRange);

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
