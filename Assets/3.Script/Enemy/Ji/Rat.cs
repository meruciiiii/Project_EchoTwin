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

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
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
