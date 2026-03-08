using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyStateAbstract
{
    [SerializeField] private float shieldDgree = 0.3f;
    [SerializeField] private float reduceRatio = 0.9f;

    protected override void Awake()
    {
        base.Awake();
        navMesh.updateRotation = true;
    }

    protected override void Update()
    {
        base.Update();
        Move();
    }
    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;

        // [추가] 공격 상태(attack)일 때는 방패 막기 판정을 아예 건너뛰고 바로 데미지를 입게 합니다.
        if (state == EnemyState.attack)
        {
            ani.ResetTrigger("Attack 2");

            currentHP -= damage;
            if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
            if (ani != null && state != EnemyState.dead) ani.SetTrigger("Hit"); // 공격 중 맞으면 피격 모션만
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
            currentHP -= damage;

            if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
            if (ani != null && state != EnemyState.dead) ani.SetTrigger("Attack 2");
        }
        else
        {
            // 일반 피격
            currentHP -= damage;

            if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
            if (ani != null && state != EnemyState.dead) ani.SetTrigger("Hit");
        }

        //currentHP -= damage;
        //checkOnDie();
    }
    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        //animator
        if (ani != null) ani.SetTrigger("Attack");
        SoundManager.SendEvent(SoundType.SFX_Skeleton);

        effect.ChargeEffect(enemyData.attackSpeed);
        yield return new WaitForSeconds(enemyData.attackSpeed);
        checkAttackTime();
        SoundManager.SendEvent(SoundType.SFX_SwordAttack2);

        AreaAttack(enemyData.attackRange, 180f);

        coroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    public override void Attack()
    {
        if (state != EnemyState.chase) return;
        if (coroutine != null) return;

        coroutine = StartCoroutine(Attack_Co());
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
