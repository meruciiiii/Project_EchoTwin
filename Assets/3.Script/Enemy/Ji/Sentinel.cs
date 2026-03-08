using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : EnemyStateAbstract
{
    [Header("RangeAttack")]
    [SerializeField] private float rangeAttackSpeed = 1f;
    [SerializeField] private int rangeAttackCount = 1;
    [SerializeField] SentinelProjectile projectilePrefab;
    private Queue<SentinelProjectile> rockPool;

    [Header("Spawn")]
    [SerializeField] private float spawnSpeed = 1f;
    [SerializeField] PebbleForBoss rangeMobPrefab;
    [SerializeField] private int rangeCount = 0;
    private Queue<PebbleForBoss> rangeMobPool;
    [SerializeField] RatForBoss meleeMobPrefab;
    [SerializeField] private int meleeCount = 0;
    private Queue<RatForBoss> meleeMobPool;
    [SerializeField] private GameObject PoolsPos;

    [Header("2ndPhaseStart")]
    [Range(0f, 1f)]
    [SerializeField] private float phaseStartHP = 0.4f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float attackSpeed = 0f;
    [SerializeField] private float coolTime = 0f;
    private bool isPhase2nd = false;

    [Header("Warning")]
    [SerializeField] private WarningGizmo warningPrefab;
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.4f);
    private Queue<WarningGizmo> warningPool;

    private List<SentinelProjectile> rockList;
    private List<PebbleForBoss> rangeMobList;
    private List<RatForBoss> meleeMobList;
    private List<WarningGizmo> warningList;
    private bool isClearPool = false;

    protected override void Awake()
    {
        base.Awake();
        if (ani == null) TryGetComponent(out ani);
        moveSpeed = enemyData.moveSpeed;
        attackSpeed = enemyData.attackSpeed;
        coolTime = enemyData.coolTime;
        setMoveSpeed();

        rockPool = new Queue<SentinelProjectile>();
        rangeMobPool = new Queue<PebbleForBoss>();
        meleeMobPool = new Queue<RatForBoss>();
        warningPool = new Queue<WarningGizmo>();

        rockList = new List<SentinelProjectile>();
        rangeMobList = new List<PebbleForBoss>();
        meleeMobList = new List<RatForBoss>();
        warningList = new List<WarningGizmo>();
        PoolsPos = Instantiate(PoolsPos);
        for (int i = 0; i < rangeAttackCount * 2; i++)
        {
            SentinelProjectile rock = Instantiate(projectilePrefab, PoolsPos.transform);
            rock.sentinel = this;
            rock.transform.localPosition = Vector3.zero;
            rock.gameObject.SetActive(false);
            rockPool.Enqueue(rock);

            rockList.Add(rock);
        }
        for (int i = 0; i < rangeCount; i++)
        {
            PebbleForBoss rangeMob = Instantiate(rangeMobPrefab, PoolsPos.transform);
            rangeMob.sentinel = this;
            rangeMob.transform.localPosition = Vector3.zero;
            rangeMob.gameObject.SetActive(false);
            rangeMobPool.Enqueue(rangeMob);

            rangeMobList.Add(rangeMob);
        }
        for (int i = 0; i < meleeCount; i++)
        {
            RatForBoss meleeMob = Instantiate(meleeMobPrefab, PoolsPos.transform);
            meleeMob.sentinel = this;
            meleeMob.transform.localPosition = Vector3.zero;
            meleeMob.gameObject.SetActive(false);
            meleeMobPool.Enqueue(meleeMob);

            meleeMobList.Add(meleeMob);
        }
        for (int i = 0; i < (rangeAttackCount + 2) * 2; i++)
        {
            WarningGizmo warning = Instantiate(warningPrefab, PoolsPos.transform);
            warning.init(returnWarning);
            warning.transform.localPosition = Vector3.zero;
            warning.gameObject.SetActive(false);
            warningPool.Enqueue(warning);

            warningList.Add(warning);
        }
    }

    protected override void OnEnable()
    {
        //state = EnemyState.chase;
        state = EnemyState.idle;
    }

    protected override void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }
        if (state == EnemyState.dead) return;

        if (ani != null)
        {
            ani.SetBool("Run", navMesh.velocity.magnitude > 0.1f);
        }

        Attack();
    }

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;

        currentHP -= damage;
        if (!isPhase2nd) checkPhaseTransition();
        if (currentHP <= 0) OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
        //if (ani != null) 
        if (state != EnemyState.dead) ani.SetTrigger("Hit");
    }

    protected override void OnDie(int goldAmount, int minCristal, int maxCristal, int minWeight, int maxWeight)
    {
        StopAllCoroutines();
        state = EnemyState.dead;
        attackCoroutine = null;

        returnAllToPool();

        reportDeadToManager();
        TurnOffNavmesh();
        rb.isKinematic = true;
        boxCol.enabled = false;

        if(PoolsPos != null)
        {
            Destroy(PoolsPos);
            PoolsPos = null;
        }

        //사망 애니메이션은 별도 루틴으로 실행 (애니메이션 시간 확보)
        StartCoroutine(DeathRoutine(goldAmount, minCristal, maxCristal, minWeight, maxWeight));
    }

    private void checkPhaseTransition()
    {
        if (!isPhase2nd && (currentHP / enemyData.maxHP) <= phaseStartHP)
        {
            start2ndPhase();
        }
    }

    private void start2ndPhase()
    {
        isPhase2nd = true;
        attackSpeed *= 0.5f;
        moveSpeed *= 1.5f;
        setMoveSpeed();
        coolTime *= 0.5f;
        rangeAttackSpeed *= 0.5f;
        rangeAttackCount *= 2;
    }

    protected override IEnumerator knockback_Co(Vector3 dir, float power)
    { yield return null; }

    protected override bool canAttack()
    {
        return Time.time >= lastAttackTime + coolTime;
    }

    public override void Attack()
    {
        if (state != EnemyState.chase) return;
        if (attackCoroutine != null) return;
        if (!canAttack())
        {
            Move();
            return;
        }

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(targetPos, startPos);

        if (distance < enemyData.attackRange - 0.5f)
        {
            if (attackCoroutine != null) return;
            attackCoroutine = StartCoroutine(Attack_Co());
        }
        else
        {
            if (attackCoroutine != null) return;
            int temp = Random.Range(0, 2);

            if (temp == 0)
            {
                attackCoroutine = StartCoroutine(RangeAttack_Co());
            }
            else if (temp == 1)
            {
                if (!isPhase2nd && (rangeMobPool.Count != rangeCount || meleeMobPool.Count != meleeCount))
                {
                    attackCoroutine = StartCoroutine(RangeAttack_Co());
                }
                else if (!isPhase2nd && rangeMobPool.Count == rangeCount && meleeMobPool.Count == meleeCount)
                {
                    attackCoroutine = StartCoroutine(MobSpawn_Co());
                }
                else
                {
                    attackCoroutine = StartCoroutine(MobSpawn_Co());
                }
            }
        }
    }

    private IEnumerator Attack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) dir = transform.forward;

        WarningGizmo warning = getWarning();
        if (warning != null)
        {
            warning.playSector(transform.position, dir.normalized, enemyData.attackRange, 270f, attackSpeed, warningColor);
        }

        yield return new WaitForSeconds(attackSpeed);

        if (ani != null) ani.SetTrigger("Attack01");

        checkAttackTime();

        AreaAttack(enemyData.attackRange, 270f);
        warning.Hide();
        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    private IEnumerator RangeAttack_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        yield return new WaitForSeconds(rangeAttackSpeed);

        if (state == EnemyState.dead || PoolsPos == null) 
        {
            attackCoroutine = null;
            yield break;
        }

        if (ani != null) ani.SetTrigger("Attack02");

        checkAttackTime();

        if (rangeAttackCount - rockPool.Count > 0)
        {
            for (int i = 0; i < rangeAttackCount - rockPool.Count; i++)
            {
                SentinelProjectile rock = Instantiate(projectilePrefab, PoolsPos.transform);
                rock.sentinel = this;
                rock.transform.localPosition = Vector3.zero;
                rock.gameObject.SetActive(false);
                rockPool.Enqueue(rock);

                rockList.Add(rock);
            }
        }

        for (int i = 0; i < rangeAttackCount; i++)
        {
            if (state == EnemyState.dead || rockPool.Count == 0)
            {
                break;
            }

            Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            Vector3 targetPos = player.transform.position + randomPos + Vector3.up * 10f;

            SentinelProjectile rock = rockPool.Dequeue();
            WarningGizmo warning = getWarning();

            rock.transform.position = targetPos;
            rock.setWarning(warning);
            rock.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);
        }

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    private IEnumerator MobSpawn_Co()
    {
        state = EnemyState.attack;

        TurnOffNavmesh();

        effect.ChargeEffect(spawnSpeed);
        yield return new WaitForSeconds(spawnSpeed);

        if (ani != null) ani.SetTrigger("Attack03");

        checkAttackTime();

        int temp = Random.Range(0, 2);

        if (!isPhase2nd)
        {
            if (temp == 0)
            {
                for (int i = 0; i < meleeCount; i++)
                {
                    Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                    Vector3 spawnPos = transform.forward * 3f + randomPos;

                    RatForBoss meleeMob = meleeMobPool.Dequeue();
                    meleeMob.transform.position = transform.position + spawnPos;
                    meleeMob.gameObject.SetActive(true);
                }
            }
            if (temp == 1)
            {
                for (int i = 0; i < rangeCount; i++)
                {
                    Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                    Vector3 spawnPos = transform.forward * 3f + randomPos;

                    PebbleForBoss rangeeMob = rangeMobPool.Dequeue();
                    rangeeMob.transform.position = transform.position + spawnPos;
                    rangeeMob.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < meleeCount; i++)
            {
                if (meleeMobPool.Count == 0) break;

                Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                Vector3 spawnPos = transform.forward * 3f + randomPos;

                RatForBoss meleeMob = meleeMobPool.Dequeue();
                meleeMob.transform.position = transform.position + spawnPos;
                meleeMob.gameObject.SetActive(true);
            }
            for (int i = 0; i < rangeCount; i++)
            {
                if (rangeMobPool.Count == 0) break;

                Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                Vector3 spawnPos = transform.forward * 3f + randomPos;

                PebbleForBoss rangeeMob = rangeMobPool.Dequeue();
                rangeeMob.transform.position = transform.position + spawnPos;
                rangeeMob.gameObject.SetActive(true);
            }
        }

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
            TurnOnNavmesh();
        }
    }

    public void returnRock(SentinelProjectile rock)
    {
        if (rock == null) return;

        if (PoolsPos != null) 
        {
            rock.transform.SetParent(PoolsPos.transform);
        }

        rock.transform.localPosition = Vector3.zero;
        rock.gameObject.SetActive(false);

        if (isClearPool || state == EnemyState.dead) return; 
        rockPool.Enqueue(rock);
    }

    public void returnRangeMob(PebbleForBoss rangeMob)
    {
        if (isClearPool || state == EnemyState.dead) return;

        rangeMob.transform.localPosition = Vector3.zero;
        rangeMob.gameObject.SetActive(false);
        rangeMobPool.Enqueue(rangeMob);
    }

    public void returnMeleeMob(RatForBoss meleeMob)
    {
        if (isClearPool || state == EnemyState.dead) return;

        meleeMob.transform.localPosition = Vector3.zero;
        meleeMob.gameObject.SetActive(false);
        meleeMobPool.Enqueue(meleeMob);
    }

    private WarningGizmo getWarning()
    {
        if (warningPool.Count == 0) return null;

        WarningGizmo warning = warningPool.Dequeue();
        warning.init(returnWarning);
        return warning;
    }

    public void returnWarning(WarningGizmo warning)
    {
        if (isClearPool || state == EnemyState.dead) return;

        warning.transform.localPosition = Vector3.zero;
        warningPool.Enqueue(warning);
    }

    private void returnAllToPool()
    {
        if (PoolsPos == null) return;
        isClearPool = true;

        for (int i = 0; i < rockList.Count; i++)
        {
            SentinelProjectile rock = rockList[i];
            if (rock == null) continue;
            rock.transform.SetParent(PoolsPos.transform);
            rock.transform.localPosition = Vector3.zero;
            if (rock.gameObject.activeSelf) rock.gameObject.SetActive(false);
        }

        for (int i = 0; i < rangeMobList.Count; i++)
        {
            PebbleForBoss rangeMob = rangeMobList[i];
            if (rangeMob == null) continue;
            rangeMob.transform.SetParent(PoolsPos.transform);
            rangeMob.transform.localPosition = Vector3.zero;
            if (rangeMob.gameObject.activeSelf) rangeMob.gameObject.SetActive(false);
        }

        for (int i = 0; i < meleeMobList.Count; i++)
        {
            RatForBoss meleeMob = meleeMobList[i];
            if (meleeMob == null) continue;
            meleeMob.transform.SetParent(PoolsPos.transform);
            meleeMob.transform.localPosition = Vector3.zero;
            if (meleeMob.gameObject.activeSelf) meleeMob.gameObject.SetActive(false);
        }

        for (int i = 0; i < warningList.Count; i++)
        {
            WarningGizmo warning = warningList[i];
            if (warning == null) continue;
            warning.transform.SetParent(PoolsPos.transform);
            warning.transform.localPosition = Vector3.zero;
            if (warning.gameObject.activeSelf) warning.gameObject.SetActive(false);
        }

        rockList.Clear();
        rangeMobList.Clear();
        meleeMobList.Clear();
        warningList.Clear();

        isClearPool = false;
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;
        if (attackCoroutine != null) return;

        setPlayerPos();
    }

    protected override void setMoveSpeed()
    {
        navMesh.speed = moveSpeed;
    }
}
