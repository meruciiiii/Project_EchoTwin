using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using Unity.AI.Navigation;

public abstract class EnemyStateAbstract : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected NavMeshModifier navMesh;

    protected float lastAttackTime;
    protected int currentHP;
    protected bool isDead;

    protected virtual void Awake()
    {
        currentHP = enemyData.maxHP;
    }

    protected virtual void takeDamage(int damage)
    {

    }

    protected virtual void knockBack()
    {

    }

    protected virtual void onDie()
    {

    }

    protected virtual bool canAttack()
    {
        return Time.time >= lastAttackTime + enemyData.coolTime;
    }

    protected virtual void checkAttackTime()
    {
        lastAttackTime = Time.time;
    }

    public abstract void Move();
    public abstract void Attack();
}
