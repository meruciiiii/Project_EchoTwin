using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyStateAbstract
{
    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Move();
    }

    public override void Attack()
    {

    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;

        BodyAttack(standardRange);

        state = EnemyState.chase;

        turnOnNavmesh();
        setPlayerPos();
    }
}
