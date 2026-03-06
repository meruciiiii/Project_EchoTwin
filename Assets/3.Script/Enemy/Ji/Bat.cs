using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bat : EnemyStateAbstract
{
    [SerializeField] private float dashSpeed = 2f;
    [SerializeField] private float zigzagRadius = 2f;
    [SerializeField] private float zigzagTime = 0.3f;

    private Vector3 zigzag;
    private float zigzagtimer;

    protected override void Update()
    {
        base.Update();
        Move();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        coroutine = StartCoroutine(Attack_Co(targetPos, startPos));
    }

    private IEnumerator Attack_Co(Vector3 destPos, Vector3 startPos)
    {
        state = EnemyState.attack;

        TurnOffNavmesh();
        SoundManager.SendEvent(SoundType.SFX_Bat);
        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        if (ani != null) ani.SetTrigger("Attack");
        //animator
        checkAttackTime();

        //bool isAttacked = false;
        Vector3 dir = (destPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, destPos);
        Vector3 targetPos = Vector3.zero;

        SoundManager.SendEvent(SoundType.SFX_DaggerThrow);
        while (distance > 0f)
        {
            //navMesh.Move(dir * enemyData.moveSpeed * bodyAttackMultiple * Time.deltaTime);
            targetPos = transform.position + (dir * enemyData.attackSpeed * Time.deltaTime);
            transform.position = targetPos;

            distance -= enemyData.attackSpeed * Time.deltaTime;

            if (dir.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }

            yield return null;
        }
        yield return new WaitForSeconds(0.2f);//애니메이션을 위한 여유시간
        transform.position = targetPos;

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

        //BodyAttack(enemyData.attackRange);

        makeZigzag();

        float distance = Vector3.Distance(player.transform.position, transform.position);
        float buffer = 0.5f;

        if (distance > enemyData.attackRange + buffer)
        {
            navMesh.SetDestination(player.transform.position + zigzag);
        }
        else
        {
            if (!canAttack()) return;
            navMesh.ResetPath();

            Attack();
        }
    }

    private void makeZigzag()
    {
        zigzagtimer -= Time.deltaTime;
        if(zigzagtimer <= 0f)
        {
            zigzagtimer = zigzagTime;

            zigzag = new Vector3(Random.Range(-zigzagRadius, zigzagRadius), 0f, Random.Range(-zigzagRadius, zigzagRadius));
        }
    }

    protected override void TurnOnNavmesh()
    {
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
        }
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            navMesh.enabled = true;
            navMesh.Warp(hit.position);
        }
        else
        {
            StartCoroutine(ReturnToField_Co());
        }
    }

    private IEnumerator ReturnToField_Co()
    {
        state = EnemyState.knockback;
        float returnSpeed = enemyData.moveSpeed * 1.5f;

        if(navMesh.enabled && navMesh.isOnNavMesh)
        {
            navMesh.isStopped = true;
            navMesh.ResetPath();
        }
        navMesh.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        while (state != EnemyState.dead)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            dir.y = 0f;
            transform.position += dir * returnSpeed * Time.deltaTime;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                navMesh.enabled = true;
                navMesh.Warp(hit.position);
                navMesh.isStopped = false;
                state = EnemyState.chase;
                yield break;
            }
            yield return null;
        }
    }

    protected override bool isItOnTheGround()
    {
        return true;
    }
}
