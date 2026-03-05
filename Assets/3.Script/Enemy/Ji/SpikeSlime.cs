using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSlime : EnemyStateAbstract
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float jumpHeight = 2f;

    protected override void Update()
    {
        base.Update();
        Move();
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
        state = EnemyState.attack;

        TurnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        if (ani != null) ani.SetTrigger("Attack");
        checkAttackTime();

        float timer = 0f;

        //bool isAttacked = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            Vector3 pos = Vector3.Lerp(startPos, destPos, t);

            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            pos.y += height;

            transform.position = pos;

            //if(!isAttacked)
            //{
            //    if (BodyAttack(enemyData.attackRange))
            //    {
            //        isAttacked = true;
            //    }
            //}

            yield return null;
        }
        SoundManager.SendEvent(SoundType.SFX_Slime);
        transform.position = destPos;

        coroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;
        if (coroutine != null) return;

        //BodyAttack(standardRange);

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
        {
            setPlayerPos();
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }
}
