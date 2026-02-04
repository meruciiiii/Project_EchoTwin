using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyStateAbstract
{
    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    public override void Attack()
    {

    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;

        state = EnemyState.chase;

        turnOnNavmesh();
        setPlayerPos();
    }
}
