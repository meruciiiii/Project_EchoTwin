using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyStateAbstract
{
    [SerializeField] private float height = 2f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private int bodyAttackMultiple = 3;
    [SerializeField] private int rangeMultiple = 2;
    [SerializeField] private GameObject projectile;
    private float fixedY;

    protected override void Awake()
    {
        base.Awake();
        fixedY = transform.position.y + height;
    }

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Attack();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (!canAttack()) Move();

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(targetPos , startPos);

        if (distance > enemyData.attackRange * rangeMultiple)
        {
            Move();
        }
        else if(distance < enemyData.attackRange * rangeMultiple && distance > enemyData.attackRange)
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(ProjectileAttack_Co(targetPos,startPos));
        }
        else
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(DashAttack_Co(targetPos, startPos));
        }

        // 플레이어와의 거리가 멀다면 원거리 공격 가까우면 근거리 공격
    }

    private IEnumerator ProjectileAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;
        turnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();

        float timer = 0f;
        float duration = 1f;

        //projectile = Instantiate<>;
        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        while (timer < duration)
        {
            if (state == EnemyState.dead)
            {
                projectile.SetActive(false);
                yield break;
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            projectile.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }
        projectile.transform.position = transform.position;
        projectile.SetActive(false);


        coroutine = null;
        turnOnNavmesh();
        state = EnemyState.chase;
    }

    private IEnumerator DashAttack_Co(Vector3 targetPos, Vector3 startPos)
    {
        state = EnemyState.attack;
        turnOffNavmesh();

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);

        bool isAttacked = false;
        Vector3 dir = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        while (distance > 0f)
        {
            //navMesh.Move(dir * enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime);
            Vector3 destPos = transform.position + (dir * enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime);
            transform.position = destPos;

            distance -= enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime;

            if (!isAttacked)
            {
                if (BodyAttack(enemyData.attackRange))
                {
                    isAttacked = true;
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);//애니메이션을 위한 여유시간
        transform.position = startPos;

        checkAttackTime();

        coroutine = null;
        turnOnNavmesh();
        state = EnemyState.chase;
    }

    public override void Move()
    {
        setPlayerPos();
    }

    protected override void setPlayerPos()
    {
        Vector3 targetPos = player.transform.position;
        targetPos.y = fixedY;
        navMesh.SetDestination(targetPos);
    }
}
