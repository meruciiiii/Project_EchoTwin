using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragon : EnemyStateAbstract
{
    protected override void OnEnable()
    {
        boxCol.isTrigger = false;
    }

    protected override void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }
        if (state == EnemyState.dead) return;

        Attack();
    }

    public override void Attack()
    {

    }

    private IEnumerator meleeAttack()
    {
        yield return null;
    }

    private IEnumerator firBreath()
    {
        yield return null;
    }

    private IEnumerator flyingBreath()
    {
        yield return null;
    }

    private IEnumerator reflection()
    {
        yield return null;
    }

    protected override void OnTriggerEnter(Collider other)
    {    }

    #region 이동관련 불필요한 method
    public override void Move()
    {    }

    protected override IEnumerator knockback_Co(Vector3 dir, float power)
    { yield return null; }

    protected override void setPlayerPos()
    {    }

    protected override void setMoveSpeed()
    {    }

    protected override void TurnOffNavmesh()
    {    }

    protected override void TurnOnNavmesh()
    {    }

    protected override bool isItOnTheGround()
    { return true; }
    #endregion
}
