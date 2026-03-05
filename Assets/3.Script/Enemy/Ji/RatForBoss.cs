using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatForBoss : EnemyStateAbstract
{
    public Sentinel sentinel;

    protected override void Awake()
    {
        base.Awake();
        sentinel = FindAnyObjectByType<Sentinel>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currentHP = enemyData.maxHP;
        state = EnemyState.idle;
    }

    protected override void Update()
    {
        base.Update();
        Move();
    }

    public override void Attack()
    {

    }

    protected override IEnumerator DeathRoutine(int goldAmount, int minCristal, int maxCristal, int minWeight, int maxWeight)
    {
        if (ani != null) ani.SetTrigger("Death");

        // 애니메이션 길이에 맞춰 대기 (예: 1.5초)
        yield return new WaitForSeconds(1.5f);

        //makeDropItem();

        sentinel.returnMeleeMob(this);
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;

        //BodyAttack(enemyData.attackRange);

        state = EnemyState.chase;

        setPlayerPos();
    }
}
