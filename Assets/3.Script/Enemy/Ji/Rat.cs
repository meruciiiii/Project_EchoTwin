using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyStateAbstract
{
    protected override void Update()
    {
        base.Update();
        Move();
    }

    public override void Attack()
    {
        SoundManager.SendEvent(SoundType.SFX_Rat);
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;

        //BodyAttack(enemyData.attackRange);

        state = EnemyState.chase;

        setPlayerPos();
    }
}
