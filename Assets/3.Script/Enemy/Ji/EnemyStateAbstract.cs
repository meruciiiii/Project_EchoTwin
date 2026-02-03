using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    chase,
    attack,
    knockback,
    dead,
}

[RequireComponent(typeof(FlashEffect))]
public abstract class EnemyStateAbstract : MonoBehaviour, Iknockback
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected NavMeshAgent navMesh;
    [SerializeField] protected PlayerStats player;
    protected FlashEffect effect;
    protected EnemyState state = EnemyState.chase;

    protected float lastAttackTime;
    protected float currentHP;

    [SerializeField] protected float knockbackTime = 0.2f;

    protected virtual void Awake()
    {
        currentHP = enemyData.maxHP;
        //player = FindAnyObjectByType<PlayerStats>();
        TryGetComponent(out effect);
        setMoveSpeed();
    }

    protected virtual void attackMotion(float time)
    {
        effect.ChargeEffect(time);
        WaitForSeconds wfs = new WaitForSeconds(time);
        //animator
    }

    public virtual void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;

        currentHP -= damage;
        checkOnDie();
    }

    protected virtual void checkOnDie()
    {
        if (currentHP <= 0)
        {
            state = EnemyState.dead;
            Destroy(gameObject);
        }
    }

    protected virtual bool canAttack()
    {
        return Time.time >= lastAttackTime + enemyData.coolTime;
    }

    protected virtual void checkAttackTime()
    {
        lastAttackTime = Time.time;
    }

    public void applyKnockback(Vector3 dir, float power)
    {
        StartCoroutine(knockback_Co(dir, power));
    }

    protected IEnumerator knockback_Co(Vector3 dir, float power)
    {
        turnOffNavmesh();
        state = EnemyState.knockback;

        float timer = knockbackTime;
        while(timer>0f)
        {
            navMesh.Move(dir * power * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(knockbackTime);

        turnOnNavmesh();
        state = EnemyState.chase;
    }

    protected virtual void setPlayerPos()
    {
        navMesh.SetDestination(player.transform.position);
    }

    protected virtual void turnOnNavmesh()
    {
        navMesh.isStopped = false;
    }

    protected virtual void turnOffNavmesh()
    {
        navMesh.isStopped = true;
    }

    protected virtual void setMoveSpeed()
    {
        navMesh.speed = enemyData.moveSpeed;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.takeDamage(enemyData.damage);
        }
    }

    public abstract void Move();
    public abstract void Attack();
}
