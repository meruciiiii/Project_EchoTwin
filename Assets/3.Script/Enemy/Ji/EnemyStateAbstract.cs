using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    idle,
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
    protected SpriteRenderer spriteRenderer;

    protected FlashEffect effect;
    public EnemyState state;

    protected float lastAttackTime;
    protected float currentHP;
    protected float radius;
    protected Vector3 lookDir = Vector3.zero;

    protected float standardRange = 0.2f;
    public float Damage => enemyData.damage;

    [SerializeField] protected float knockbackTime = 0.2f;
    [SerializeField] protected LayerMask ground;

    protected Coroutine coroutine;

    protected AttackDebugInfo bodyAttackInfo;
    protected bool hasBodyAttackDebug;
    protected AttackDebugInfo areaAttackInfo;
    protected bool hasAreaAttackDebug;

    private List<AttackDebugInfo> debugInfoList = new List<AttackDebugInfo>();

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
        TryGetComponent(out spriteRenderer);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gizmo.enemy = this;
        setMoveSpeed();
        radius = boxCol.size.x * 0.5f;
        boxCol.isTrigger = true;

        ani = GetComponentInChildren<Animator>();
        TryGetComponent(out rb);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        player = FindAnyObjectByType<PlayerAction>();
        //state = EnemyState.idle;
        state = EnemyState.chase;
    }

    protected virtual void OnEnable()
    {
        FixedRotation();
    }

    protected virtual void Update()
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

        if (navMesh.enabled && navMesh.desiredVelocity.sqrMagnitude > 0.01f)
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
        //if (ani != null) 
        if (state != EnemyState.dead) ani.SetTrigger("Hit");
    }

    protected virtual void checkOnDie()
    {
        if (currentHP <= 0)
        {
            StopAllCoroutines();
            state = EnemyState.dead;


            TurnOffNavmesh();
            rb.isKinematic = true;
            boxCol.enabled = false;

            //사망 애니메이션은 별도 루틴으로 실행 (애니메이션 시간 확보)
            StartCoroutine(DeathRoutine());
        }
    }
    private IEnumerator DeathRoutine()
    {
        if (ani != null) ani.SetTrigger("Death");

        // 애니메이션 길이에 맞춰 대기 (예: 1.5초)
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
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
        if (state == EnemyState.dead) return;
        if (state == EnemyState.knockback) return;
        StartCoroutine(knockback_Co(dir, power));
    }

    protected virtual IEnumerator knockback_Co(Vector3 dir, float power)
    {
        state = EnemyState.knockback;

        TurnOffNavmesh();
        navMesh.enabled = false;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 force = dir * power;
        force.y = 0f;

        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        if (isItOnTheGround())
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

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
        if (navMesh.enabled && navMesh.isOnNavMesh)
        {
            navMesh.isStopped = true;
            navMesh.ResetPath();
            //navMesh.enabled = false;
        }
        //else
        //{
        //    navMesh.enabled = false;
        //}

        //rb.isKinematic = false;
        //rb.linearVelocity = Vector3.zero;
    }

    protected virtual void TurnOnNavmesh()
    {
        if (state == EnemyState.dead) return;
        //navMesh.isStopped = false;

        //rb.isKinematic = false;
        //rb.linearVelocity = Vector3.zero;
        //rb.isKinematic = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            navMesh.enabled = true;
            navMesh.Warp(hit.position);
            navMesh.isStopped = false;
        }
        else
        {
            //navMesh.enabled = false;
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

    //protected virtual void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        player.takeDamage(enemyData.damage, transform.position);
    //    }
    //}
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
        float progress = Mathf.Clamp01(currentTime / maxTime);

        areaAttackInfo = new AttackDebugInfo
        {
            shape = AttackShape.sector,
            center = transform.position,
            size = new Vector3(range, 0, 0),
            direction = transform.forward,
            color = Color.Lerp(Color.yellow, Color.red, progress),
            ratio = progress
        };
        hasAreaAttackDebug = true;
    }

    //protected bool BodyAttack(float range)
    //{
    //    float checkRadius = radius + range;

    //    bodyAttackInfo = new AttackDebugInfo
    //    {
    //        shape = AttackShape.sphere,
    //        center = transform.position,
    //        size = Vector3.one * checkRadius,
    //        rotation = Quaternion.identity,
    //        color = Color.gray,
    //        ratio = 1f
    //    };
    //    hasBodyAttackDebug = true;

    //    Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
    //    foreach (Collider hit in hits)
    //    {
    //        if (hit.CompareTag("Player"))
    //        {
    //            player.takeDamage(enemyData.damage, transform.position);
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player.takeDamage(enemyData.damage, transform.position);
        }
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

                if (Vector3.Angle(lookDir, dirToTarget) <= angle * 0.5f)
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
