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

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            player.takeDamage(enemyData.damage, transform.position, 1);
            Attack();
        }
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
