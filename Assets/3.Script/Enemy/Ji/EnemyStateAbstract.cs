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
    [SerializeField] protected PlayerAction player;

    AttackDebugGizmo gizmo;

    protected FlashEffect effect;
    protected EnemyState state = EnemyState.chase;

    protected float lastAttackTime;
    protected float currentHP;
    protected float radius;

    protected float standardRange = 1f;

    [SerializeField] protected float knockbackTime = 0.2f;

    protected Coroutine coroutine;

    protected AttackDebugInfo lastAttackInfo;
    protected bool hasDebugInfo;

    public AttackDebugInfo DebugInfo => lastAttackInfo;
    public bool HasDebugInfo => hasDebugInfo;


    protected virtual void Awake()
    {
        currentHP = enemyData.maxHP;
        //player = FindAnyObjectByType<PlayerStats>();
        TryGetComponent(out effect);
        TryGetComponent(out gizmo);
        gizmo.enemy = this;
        setMoveSpeed();
        radius = transform.GetComponent<CapsuleCollider>().radius;
    }

    protected virtual void Update()
    {
        if (GameManager.instance.isStop)
        {
            turnOffNavmesh();
            return;
        }
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
        while (timer > 0f)
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
        if (other.CompareTag("Player"))
        {
            player.takeDamage(enemyData.damage, transform.position);
        }
    }

    protected bool BodyAttack(float range)
    {
        float checkRadius = radius + range;

        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
        foreach(Collider hit in hits)
        {
            if(hit.CompareTag("Player"))
            {
                player.takeDamage(enemyData.damage, transform.position);
                return true;
            }
        }
        return false;
    }

    protected void AreaAttack(float range , float angle)
    {
        lastAttackInfo = new AttackDebugInfo
        {
            center = transform.position,
            halfExtents = Vector3.one * range,
            rotation = Quaternion.identity,
            color = Color.magenta,
            angle = angle,
            direction = transform.forward
        };
        hasDebugInfo = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        foreach(Collider hit in hits)
        {
            if(hit.CompareTag("Player"))
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                dirToTarget.y = 0f;
                Vector3 forward = transform.forward;
                forward.y = 0f;

                if(Vector3.Angle(forward, dirToTarget) <= angle * 0.5f)
                {
                    player.takeDamage(enemyData.damage, transform.position);
                }
            }
        }    
    }

    public abstract void Move();
    public abstract void Attack();
}
