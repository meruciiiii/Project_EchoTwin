using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pebble : EnemyStateAbstract
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float buffer = 0.5f;
    [SerializeField] private float projectileDuration = 1f;
    [SerializeField] private float projectileSpeed = 10f;

    [SerializeField] private float sideWalkTime = 0.5f;
    private float sideTimer;
    private int sign = 1;

    protected override void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }
        if (state == EnemyState.dead) return;

        // 속도가 있으면 Run, 없으면 Idle
        if (ani != null)
        {
            ani.SetBool("Run", navMesh.velocity.magnitude > 0.1f);
        }

        if ((player.transform.position - transform.position).normalized.x > 0.01)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        Attack();
    }

    public override void Attack()
    {
        if (state != EnemyState.chase) return;
        if (attackCoroutine != null) return;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (!canAttack() || distance > enemyData.attackRange + buffer)
        {
            Move();
            return;
        }

        attackCoroutine = StartCoroutine(Attack_Co());
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        if (navMesh.enabled && navMesh.isOnNavMesh)
        {
            navMesh.ResetPath();
        }

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        //animator
        if (ani != null) ani.SetTrigger("Attack");
        SoundManager.SendEvent(SoundType.SFX_Skul);

        checkAttackTime();

        Vector3 startPos = transform.position;
        startPos.y += 1.5f; // 몬스터의 피벗이 발밑이라면 0.5f 정도 올려서 가슴 위치로 잡음

        Vector3 targetPos = player.transform.position;
        //targetPos.y -= 0.5f; // 플레이어도 발밑이 아닌 몸 중심을 조준하도록 수정

        Vector3 dir = (targetPos - startPos).normalized;

        float timer = 0f;

        projectile.transform.position = startPos;
        projectile.SetActive(true);
        //if (spriteRenderer.flipX)
        //{
        //    projectile.GetComponentInChildren<SpriteRenderer>().flipX = true;
        //}
        //if (!spriteRenderer.flipX)
        //{
        //    projectile.GetComponentInChildren<SpriteRenderer>().flipX = false;
        //}

        while (timer < projectileDuration)
        {
            if (state == EnemyState.dead)
            {
                yield break;
            }

            if (!projectile.activeSelf)
            {
                projectile.transform.position = startPos;
                projectile.SetActive(false);

                attackCoroutine = null;

                if (state != EnemyState.dead)
                {
                    state = EnemyState.chase;
                    TurnOnNavmesh();
                }
                yield break;
            }

            timer += Time.deltaTime;
            float t = timer / projectileDuration;
            projectile.transform.position += dir * projectileSpeed * Time.deltaTime;

            yield return null;
        }
        projectile.transform.position = startPos;
        projectile.SetActive(false);

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;
        if (attackCoroutine != null) return;

        //BodyAttack(standardRange);

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance > enemyData.attackRange + buffer)
        {
            setPlayerPos();
        }
        else if (distance < enemyData.attackRange - buffer)
        {
            Runaway();
        }
        else
        {
            SideWalk();
        }
    }

    private void SideWalk()
    {
        sideTimer -= Time.deltaTime;
        if (sideTimer > 0f) return;
        sideTimer = sideWalkTime;

        sign *= -1;

        Vector3 sideDir = Vector3.Cross(Vector3.up, (transform.position - player.transform.position).normalized) * sign;
        sideDir.y = 0;
        Vector3 sidePos = transform.position + sideDir * 2f;
        sidePos.y = 0;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(sidePos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }

    private void Runaway()
    {
        Vector3 dir = transform.position - player.transform.position;
        dir.y = transform.position.y;
        Vector3 runPos = transform.position + dir.normalized * 2f;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(runPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }

    protected override void TurnOffNavmesh()
    {
        if (navMesh != null && navMesh.enabled && navMesh.isOnNavMesh)
        {
            navMesh.isStopped = true;
            navMesh.ResetPath();
        }

        rb.isKinematic = false;
        //rb.linearVelocity = Vector3.zero;
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

        if (navMesh.enabled && navMesh.isOnNavMesh)
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
