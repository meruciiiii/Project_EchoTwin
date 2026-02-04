using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : EnemyStateAbstract
{
    [SerializeField] private float height = 2f;
    [SerializeField] private float duration = 0.5f;
    private Coroutine coroutine;
    private float fixedY;

    protected override void Awake()
    {
        base.Awake();
        fixedY = transform.position.y + height;
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        state = EnemyState.attack;

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        coroutine = StartCoroutine(Attack_Co(targetPos, startPos));
    }

    private IEnumerator Attack_Co(Vector3 destPos, Vector3 startPos)
    {
        float timer = 0f;

        turnOffNavmesh();

        checkAttackTime();

        attackMotion(enemyData.attackSpeed);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.position = Vector3.Lerp(startPos, destPos, t);

            yield return null;
        }
        transform.position = startPos;

        state = EnemyState.chase;
        coroutine = null;

        turnOnNavmesh();
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
        {
            state = EnemyState.chase;
            setPlayerPos();
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }

    protected override void setPlayerPos()
    {
        Vector3 targetPos = player.transform.position;
        targetPos.y = fixedY;
        navMesh.SetDestination(targetPos);
    }
}
