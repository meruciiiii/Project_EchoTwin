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
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AttackDebugGizmo))]
public abstract class EnemyStateAbstract : MonoBehaviour, Iknockback
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected NavMeshAgent navMesh;
    [SerializeField] protected PlayerAction player;

    protected AttackDebugGizmo gizmo;
    protected Animator ani;
    protected Rigidbody rb;
    protected BoxCollider boxCol;

    protected FlashEffect effect;
    protected EnemyState state = EnemyState.chase;

    protected float lastAttackTime;
    protected float currentHP;
    protected float radius;
    protected Vector3 lookDir = Vector3.zero;

    protected float standardRange = 0.2f;
    public float Damage => enemyData.damage;

    [SerializeField] protected float knockbackTime = 0.2f;
    [SerializeField] protected LayerMask ground;

    protected Coroutine coroutine;

    protected List<AttackDebugInfo> debugInfoList = new List<AttackDebugInfo>();

    protected AttackDebugInfo bodyAttackInfo;
    protected bool hasBodyAttackDebug;

    protected AttackDebugInfo areaAttackInfo;
    protected bool hasAreaAttackDebug;

    public AttackDebugInfo BodyAttackInfo => bodyAttackInfo;
    public bool HasBodyAttackDebug => hasBodyAttackDebug;
    public AttackDebugInfo AreaAttackInfo => areaAttackInfo;
    public bool HasAreaAttackDebug => hasAreaAttackDebug;

    protected virtual void Awake()
    {
        currentHP = enemyData.maxHP;
        //player = FindAnyObjectByType<PlayerStats>();
        TryGetComponent(out effect);
        TryGetComponent(out gizmo);
        TryGetComponent(out boxCol);
        gizmo.enemy = this;
        setMoveSpeed();
        radius = boxCol.size.x * 0.5f;
        boxCol.isTrigger = false;

        ani = GetComponentInChildren<Animator>();
        TryGetComponent(out rb);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    protected virtual void OnEnable()
    {
        FixedRotation();
        setPlayerPos();
    }

    protected virtual void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }

        //if (navMesh.enabled)
        //{
        //    if (navMesh.desiredVelocity.x > 0.1f)
        //    {
        //        GetComponentInChildren<SpriteRenderer>().flipX = false;
        //    }
        //    else if (navMesh.desiredVelocity.x < -0.1f)
        //    {
        //        GetComponentInChildren<SpriteRenderer>().flipX = true;
        //    }
        //}

        if(navMesh.enabled && navMesh.desiredVelocity.sqrMagnitude>0.01f)
        {
            lookDir = navMesh.desiredVelocity.normalized;
            lookDir.y = 0f;

            GetComponentInChildren<SpriteRenderer>().flipX = (lookDir.x < -0.1f);
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
            TurnOffNavmesh();
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

    protected virtual IEnumerator knockback_Co(Vector3 dir, float power)
    {
        state = EnemyState.knockback;

        TurnOffNavmesh();

        Vector3 force = dir * power;
        force.y = 0f;

        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        if (isItOnTheGround())
        {
            TurnOnNavmesh();
            state = EnemyState.chase;
        }
    }

    #region navMesh
    protected virtual void setPlayerPos()
    {
        navMesh.SetDestination(player.transform.position);
    }

    protected virtual void TurnOffNavmesh()
    {
        navMesh.enabled = false;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
    }

    protected virtual void TurnOnNavmesh()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            navMesh.enabled = true;
            navMesh.Warp(hit.position);
        }
        else
        {
            state = EnemyState.dead;
        }
    }

    protected void UnFixedRotation()
    {
        navMesh.updateRotation = true;
        navMesh.updateUpAxis = true;
    }

    protected void FixedRotation()
    {
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
    }

    protected virtual bool isItOnTheGround()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 1.5f, ground);
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
    #endregion

    #region attack

    public List<AttackDebugInfo> getAllDebugInfo()
    {
        debugInfoList.Clear();
        if (hasBodyAttackDebug) debugInfoList.Add(bodyAttackInfo);
        if (hasAreaAttackDebug) debugInfoList.Add(areaAttackInfo);

        return debugInfoList;
    }

    protected void updateBossCharging(float currentTime, float maxTime, float range, float angle)
    {
        //coroutine¿∏∑Œ currentTime ¿ª ¥√∑¡¡‡æﬂ «‘
        float progress = Mathf.Clamp01(currentTime / maxTime);

        areaAttackInfo = new AttackDebugInfo
        {
            shape = AttackShape.sector,
            center = transform.position,
            size = new Vector3(range,0,0),
            angle = angle,
            direction = lookDir,
            color = Color.Lerp(Color.yellow,Color.red,progress),
            ratio = progress
        };
        hasAreaAttackDebug = true;
    }

    protected bool BodyAttack(float range)
    {
        float checkRadius = radius + range;

        bodyAttackInfo = new AttackDebugInfo
        {
            shape = AttackShape.sphere,
            center = transform.position,
            size = Vector3.one * checkRadius,
            rotation = Quaternion.identity,
            color = Color.gray,
            ratio = 1f
        };
        hasBodyAttackDebug = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player.takeDamage(enemyData.damage, transform.position);
                return true;
            }
        }
        return false;
    }

    protected void AreaAttack(float range, float angle)
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0f;
        areaAttackInfo = new AttackDebugInfo
        {
            shape = AttackShape.sector,
            center = transform.position,
            size = new Vector3(range, 0, 0),
            rotation = Quaternion.identity,
            color = Color.magenta,
            angle = angle,
            direction = lookDir,
            ratio = 1f
        };
        hasAreaAttackDebug = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                dirToTarget.y = 0f;
                //Vector3 forward = transform.forward;
                Vector3 forward = navMesh.desiredVelocity;
                forward.y = 0f;

                if (Vector3.Angle(forward, dirToTarget) <= angle * 0.5f)
                {
                    player.takeDamage(enemyData.damage, transform.position);
                }
            }
        }
    }
    #endregion

    public abstract void Move();
    public abstract void Attack();
}
