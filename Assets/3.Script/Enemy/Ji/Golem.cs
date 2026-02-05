using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyStateAbstract
{
    [SerializeField] private float height = 2f;
    private float fixedY;

    public override void Attack()
    {
        if (!canAttack()) Move();

        //if()
    }

    public override void Move()
    {
        setPlayerPos();
    }

    protected override void setPlayerPos()
    {
        //base.setPlayerPos();
        Vector3 targetPos = player.transform.position;
        targetPos.y = fixedY;
        navMesh.SetDestination(targetPos);
    }
}
