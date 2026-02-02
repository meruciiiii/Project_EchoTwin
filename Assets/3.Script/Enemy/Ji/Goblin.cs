using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : EnemyStateAbstract
{
    private Coroutine coroutine;
    [SerializeField] float attackSpeed = 10f;
    [SerializeField] float dashDuration = 0.5f;

    private void Update()
    {
        if (state == EnemyState.dead) return;
        Move();
    }

    private IEnumerator attack_Co(float attackTime)
    {
        state = EnemyState.attack;

        //animator Â÷Â¡ ¶Ç´Â ¸ØÃá ¸ð¼Ç
        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(attackTime);

        Vector3 dir = (player.transform.position - transform.position).normalized;

        float timer = dashDuration;
        while (timer > 0f)
        {
            navMesh.Move(dir * attackSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        checkAttackTime();

        state = EnemyState.chase;
        coroutine = null;
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
        if (coroutine != null) return;

        state = EnemyState.chase;

        float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
        if (sqrDist <= enemyData.attackRange * enemyData.attackRange)
        {
            if (!canAttack()) return;
            turnOffNavmesh();
            coroutine = StartCoroutine(attack_Co(enemyData.attackSpeed));
        }
        else
        {
            turnOnNavmesh();
            setPlayerPos();
        }
    }
}
