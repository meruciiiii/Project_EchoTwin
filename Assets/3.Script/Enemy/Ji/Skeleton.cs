using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyStateAbstract
{
    [SerializeField] private float shieldDgree = 0.3f;
    [SerializeField] private float reduceRatio = 0.9f;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        navMesh.updateRotation = true;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.dead) return;
        Move();
    }
    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;

        // [추가] 공격 상태(attack)일 때는 방패 막기 판정을 아예 건너뛰고 바로 데미지를 입게 합니다.
        if (state == EnemyState.attack)
        {
            ani.ResetTrigger("Attack 2");

            if (ani != null) ani.SetTrigger("Hit"); // 공격 중 맞으면 피격 모션만
            effect.Flash(1, 0.5f);
            currentHP -= damage;
            checkOnDie();
            return; // 여기서 함수 종료 (아래의 방패 막기 로직 실행 안 함)
        }

        Vector3 attackerPos = player.transform.position;
        Vector3 dir = (transform.position - attackerPos).normalized;

        // 1. Flip 상태 확인 (Scale 또는 SpriteRenderer)
        bool isFlipped = transform.lossyScale.x < 0 || (spriteRenderer != null && spriteRenderer.flipX);

        // 2. [반전 적용] 
        // 기본(오른쪽)일 때 -right를 보고, 플립(왼쪽)일 때 +right를 보게 설정
        Vector3 visualForward = isFlipped ? transform.right : -transform.right;

        float dot = Vector3.Dot(visualForward, dir);

        // [시각화] 파란 선이 방패 쪽을 향하는지 꼭 확인!
        Debug.DrawRay(transform.position, visualForward * 2f, Color.blue, 1f);
        Debug.Log($"Flipped: {isFlipped}, Dot: {dot}");

        if (dot > shieldDgree)
        {
            // 방패 막기
            damage *= 1 - reduceRatio;
            if (ani != null) ani.SetTrigger("Attack 2");
            effect.Flash(1, 0.5f);
        }
        else
        {
            // 일반 피격
            if (ani != null) ani.SetTrigger("Hit");
            effect.Flash(1, 0.5f);
            state = EnemyState.knockback;
        }

        currentHP -= damage;
        checkOnDie();
    }
    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        //animator
        if (ani != null) ani.SetTrigger("Attack");

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();

        AreaAttack(enemyData.attackRange, 180f);

        coroutine = null;

        TurnOnNavmesh();
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (coroutine != null) return;

        coroutine = StartCoroutine(Attack_Co());
    }

    public override void Move()
    {
        if (state == EnemyState.knockback) return;
        if (coroutine != null) return;

        //BodyAttack(standardRange);

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
}
