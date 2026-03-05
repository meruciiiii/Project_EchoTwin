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
        Move();
    }

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;
        if (shieldCount > 0)
        {
            damage *= 1 - reduceRatio;

            currentHP -= damage;
            shieldCount--;
            if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
            if (ani != null && state != EnemyState.dead) ani.SetTrigger("Ability");
        }
        else
        {
            // 껍데기가 깨진 후에는 일반적인 피격 애니메이션 실행
            currentHP -= damage;
            if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
            if (ani != null && state != EnemyState.dead) ani.SetTrigger("Hit");
        }

        //currentHP -= damage;
        //shieldCount--;
        //checkOnDie();
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
        SoundManager.SendEvent(SoundType.SFX_Crab);
        //animator
        if (ani != null) ani.SetTrigger("Attack");

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();

        AreaAttack(enemyData.attackRange, 180f);

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
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
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
