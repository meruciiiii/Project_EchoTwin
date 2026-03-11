using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragon : EnemyStateAbstract
{
    private MeshRenderer meshRenderer;
    private BoxCollider[] cols;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] private GameObject breathFx;

    [Header("MeleeAttack")]
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    private float attackRange = 0f;

    [Header("Breath")]
    [SerializeField] private GameObject head;
    [SerializeField] private float breathDistance;
    [SerializeField] private float breathAngle;
    [SerializeField]
    [Range(0f, 1f)] private float hitStart = 0.3f;
    [SerializeField]
    [Range(0f, 1f)] private float hitEnd = 0.8f;
    [SerializeField] private float breathTotalAngle = 180f;
    [SerializeField] private Color breathBaseColor = new Color(1f, 0f, 0f, 0.2f);
    [SerializeField] private Color breathFillColor = new Color(1f, 0f, 0f, 0.45f);

    [SerializeField] private Transform breathOrigin;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckHeight = 10f;
    [SerializeField] private float groundCheckDistance = 30f;
    [SerializeField] private float breathHitInterval = 0.15f;

    private float lastBreathHitTime = -Mathf.Infinity;

    [Header("Reflection")]
    [SerializeField] private float reflectionTime = 1f;
    [SerializeField] private float reflectKnockbackForce = 3f;
    private bool isReflect = false;
    [Range(0f, 1f)] [SerializeField] private float stopRate = 0.5f;
    private bool reflectResumeRequested = false;

    [Header("RangeAttack")]
    [SerializeField] private float rangeAttackSpeed = 1f;
    [SerializeField] private int rangeAttackCount = 1;
    [SerializeField] DragonProjectile projectilePrefab;
    private Queue<DragonProjectile> rockPool;
    [SerializeField] DragonFireArea fireAreaPrefab;
    private Queue<DragonFireArea> areaPool;
    [SerializeField] GameObject PoolsPos;
    private bool isFlying = false;

    [Header("Phase2nd")]
    [Range(0f, 1f)]
    [SerializeField] private float phaseStartHP = 0.4f;
    [SerializeField] private float attackSpeed = 0f;
    [SerializeField] private float coolTime = 0f;
    private bool isPhase2 = false;

    [Header("Warning")]
    [SerializeField] private WarningGizmo warningPrefab;
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.4f);
    private Queue<WarningGizmo> warningPool;

    private int meleeWeight = 40;
    private int breathWeight = 40;
    private int reflectWeight = 20;
    private int rangeWeight = 20;

    private enum AttackPattern
    {
        none,
        melee,
        breath,
        reflect,
        range,
    }

    private AttackPattern lastPattern;

    protected override void Awake()
    {
        currentHP = enemyData.maxHP;
        player = FindAnyObjectByType<PlayerAction>();

        TryGetComponent(out effect);
        TryGetComponent(out gizmo);
        TryGetComponent(out boxCol);

        cols = GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider col in cols)
        {
            if (col == null) continue;
            col.isTrigger = true;
        }

        if (boxCol != null)
        {
            boxCol.isTrigger = true;
            boxCol.enabled = false;
        }

        gizmo.enemy = this;
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (breathFx != null) breathFx.SetActive(false);

        ani = GetComponentInChildren<Animator>();

        if (player != null) stats = player.GetComponent<PlayerStats>();

        state = EnemyState.chase;

        if (ani == null) TryGetComponent(out ani);

        attackRange = enemyData.attackRange;
        attackSpeed = enemyData.attackSpeed;
        coolTime = enemyData.coolTime;

        rockPool = new Queue<DragonProjectile>();
        areaPool = new Queue<DragonFireArea>();
        warningPool = new Queue<WarningGizmo>();

        PoolsPos = Instantiate(PoolsPos);

        for (int i = 0; i < rangeAttackCount * 2; i++)
        {
            DragonProjectile rock = Instantiate(projectilePrefab, PoolsPos.transform);
            rock.dragon = this;
            rock.transform.localPosition = Vector3.zero;
            rock.gameObject.SetActive(false);
            rockPool.Enqueue(rock);
        }

        for (int i = 0; i < rangeAttackCount * 4; i++)
        {
            DragonFireArea area = Instantiate(fireAreaPrefab, PoolsPos.transform);
            area.dragon = this;
            area.transform.localPosition = Vector3.zero;
            area.gameObject.SetActive(false);
            areaPool.Enqueue(area);
        }

        for (int i = 0; i < (rangeAttackCount + 2) * 4; i++)
        {
            WarningGizmo warning = Instantiate(warningPrefab, PoolsPos.transform);
            warning.init(returnWarning);
            warning.transform.localPosition = Vector3.zero;
            warning.gameObject.SetActive(false);
            warningPool.Enqueue(warning);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (cols != null)
        {
            foreach (BoxCollider col in cols)
            {
                if (col == null) continue;
                if (col == boxCol) continue;
                col.enabled = true;
            }
        }

        if (boxCol != null)
        {
            boxCol.enabled = false;
        }

        isReflect = false;
        reflectResumeRequested = false;
        isFlying = false;
        isPhase2 = false;
        lastPattern = AttackPattern.none;

        if (ani != null) ani.speed = 1f;
    }

    public override void takeDamage(float damage)
    {
        if (state == EnemyState.dead) return;
        if (isFlying) return;

        if (isReflect)
        {
            reflectResumeRequested = true;
            player.takeDamage(enemyData.damage, transform.position, reflectKnockbackForce);
            return;
        }

        currentHP -= damage;

        if (!isPhase2) checkPhaseTransition();

        if (currentHP <= 0)
        {
            OnDie(enemyData.dropGold, enemyData.minCristal, enemyData.maxCristal, enemyData.minWeight, enemyData.maxWeight);
        }

        if (state != EnemyState.dead && ani != null)
        {
            ani.SetTrigger("Hit");
        }
    }

    protected override void OnDie(int goldAmount, int minCristal, int maxCristal, int minWeight, int maxWeight)
    {
        if (boxCol != null) boxCol.enabled = false;

        if (cols != null)
        {
            foreach (BoxCollider col in cols)
            {
                if (col == null) continue;
                col.enabled = false;
            }
        }

        base.OnDie(goldAmount, minCristal, maxCristal, minWeight, maxWeight);
    }

    private void checkPhaseTransition()
    {
        if (!isPhase2 && (currentHP / enemyData.maxHP) <= phaseStartHP)
        {
            start2ndPhase();
        }
    }

    private void start2ndPhase()
    {
        isPhase2 = true;
        attackSpeed *= 0.5f;
        attackRange *= 1.5f;
        coolTime *= 0.5f;
        rangeAttackSpeed *= 0.5f;
        rangeAttackCount *= 2;
    }

    protected override bool canAttack()
    {
        return Time.time >= lastAttackTime + coolTime;
    }

    protected override void Update()
    {
        if (GameManager.instance.isStop)
        {
            TurnOffNavmesh();
            return;
        }

        if (state == EnemyState.dead) return;

        Attack();
    }

    public override void Attack()
    {
        if (state != EnemyState.chase) return;
        if (attackCoroutine != null) return;
        if (!canAttack()) return;

        AttackPattern pattern;
        if (!selectPattern(out pattern)) return;

        lastPattern = pattern;

        switch (pattern)
        {
            case AttackPattern.melee:
                attackCoroutine = StartCoroutine(meleeAttack_Co());
                break;

            case AttackPattern.breath:
                attackCoroutine = StartCoroutine(fireBreath_Co());
                break;

            case AttackPattern.reflect:
                attackCoroutine = StartCoroutine(reflection_Co());
                break;

            case AttackPattern.range:
                attackCoroutine = StartCoroutine(rangeAttack_Co());
                break;
        }
    }

    private bool selectPattern(out AttackPattern pattern)
    {
        pattern = AttackPattern.none;

        int melee = meleeWeight;
        int breath = breathWeight;
        int reflect = reflectWeight;
        int range = isPhase2 ? rangeWeight : 0;

        int availableCount = 0;

        if (melee > 0) availableCount++;
        if (breath > 0) availableCount++;
        if (reflect > 0) availableCount++;
        if (range > 0) availableCount++;

        if (availableCount == 0) return false;

        if (availableCount > 1)
        {
            switch (lastPattern)
            {
                case AttackPattern.melee:
                    melee = 0;
                    break;

                case AttackPattern.breath:
                    breath = 0;
                    break;

                case AttackPattern.reflect:
                    reflect = 0;
                    break;

                case AttackPattern.range:
                    range = 0;
                    break;
            }
        }

        int totalWeight = melee + breath + reflect + range;
        if (totalWeight <= 0) return false;

        int randomValue = Random.Range(0, totalWeight);

        if (randomValue < melee)
        {
            pattern = AttackPattern.melee;
            return true;
        }
        randomValue -= melee;

        if (randomValue < breath)
        {
            pattern = AttackPattern.breath;
            return true;
        }
        randomValue -= breath;

        if (randomValue < reflect)
        {
            pattern = AttackPattern.reflect;
            return true;
        }

        pattern = AttackPattern.range;
        return true;
    }

    private IEnumerator meleeAttack_Co()
    {
        state = EnemyState.attack;

        WarningGizmo leftWarning = getWarning();
        WarningGizmo rightWarning = getWarning();

        if (leftWarning != null)
        {
            leftWarning.playCircle(leftArm.transform.position, attackRange, attackSpeed, warningColor);
        }

        if (rightWarning != null)
        {
            rightWarning.playCircle(rightArm.transform.position, attackRange, attackSpeed, warningColor);
        }

        if (ani != null) ani.SetTrigger("Attack01");

        yield return new WaitForSeconds(attackSpeed);

        checkAttackTime();

        Collider[] lefthits = Physics.OverlapSphere(leftArm.transform.position, attackRange, playerLayer);
        Collider[] righthits = Physics.OverlapSphere(rightArm.transform.position, attackRange, playerLayer);

        foreach (Collider hit in lefthits)
        {
            PlayerAction target = hit.GetComponentInParent<PlayerAction>();
            if (target == null) continue;

            target.takeDamage(enemyData.damage, leftArm.transform.position, 2);
            break;
        }

        foreach (Collider hit in righthits)
        {
            PlayerAction target = hit.GetComponentInParent<PlayerAction>();
            if (target == null) continue;

            target.takeDamage(enemyData.damage, rightArm.transform.position, 2);
            break;
        }

        if (leftWarning != null) leftWarning.Hide();
        if (rightWarning != null) rightWarning.Hide();

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
        }
    }

    private Transform getBreathAnchor() 
    {
        if (breathOrigin != null) return breathOrigin;
        if (head != null) return head.transform;
        return transform;
    }

    private Vector3 getBreathGroundPos()
    {
        Transform anchor = getBreathAnchor();
        Vector3 worldPos = anchor.position;
        Vector3 rayStart = worldPos + Vector3.up * groundCheckHeight;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckHeight + groundCheckDistance, groundLayer))
        {
            return hit.point;
        }

        return new Vector3(worldPos.x, transform.position.y, worldPos.z);
    }

    private Vector3 getFlatForward(Vector3 forward)
    {
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            Vector3 fallback = transform.forward;
            fallback.y = 0f;

            if (fallback.sqrMagnitude < 0.001f)
            {
                fallback = Vector3.forward;
            }

            return fallback.normalized;
        }

        return forward.normalized;
    }

    private Vector3 getBreathFlatDir()
    {
        Transform anchor = getBreathAnchor();
        return getFlatForward(-anchor.right);
    }

    private void syncBreathFxTransform() 
    {
        if (breathFx == null) return;

        Transform anchor = getBreathAnchor();
        breathFx.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
    }

    private void setBreathFxActive(bool value) 
    {
        if (breathFx == null) return;

        if (value)
        {
            syncBreathFxTransform();
        }

        if (breathFx.activeSelf != value)
        {
            breathFx.SetActive(value);
        }
    }

    private void showBreathWarnings(WarningGizmo baseWarning, WarningGizmo fillWwarning, Vector3 fixedWarningPos, Vector3 fixedBaseDir, Vector3 currentFillDir)
    {
        if (baseWarning != null)
        {
            baseWarning.showSector(fixedWarningPos, fixedBaseDir, breathDistance, breathTotalAngle, breathBaseColor);
        }

        if (fillWwarning != null)
        {
            fillWwarning.showSector(fixedWarningPos, currentFillDir, breathDistance, breathAngle * 2f, breathFillColor);
        }
    }

    private void hideBreathWarnings(WarningGizmo baseWarning, WarningGizmo fillWwarning)
    {
        if (baseWarning != null) baseWarning.Hide();
        if (fillWwarning != null) fillWwarning.Hide();
    }

    private bool isPlayerInBreath(Vector3 origin, Vector3 centerDir)
    {
        if (player == null) return false;

        Vector3 toPlayer = player.transform.position - origin;
        toPlayer.y = 0f;

        float sqrDistance = toPlayer.sqrMagnitude;
        if (sqrDistance > breathDistance * breathDistance) return false;

        if (sqrDistance < 0.0001f) return true;

        float angle = Vector3.Angle(centerDir, toPlayer.normalized);
        return angle <= breathAngle;
    }

    private void tryBreathDamage(Vector3 origin, Vector3 centerDir)
    {
        if (player == null) return;
        if (Time.time < lastBreathHitTime + breathHitInterval) return;
        if (!isPlayerInBreath(origin, centerDir)) return;

        lastBreathHitTime = Time.time;
        player.takeDamage(enemyData.damage, transform.position, 1f);
    }

    private IEnumerator fireBreath_Co() 
    {
        state = EnemyState.attack;

        WarningGizmo baseWarning = getWarning();
        WarningGizmo fillWwarning = getWarning();

        Vector3 fixedWarningPos = getBreathGroundPos();
        Vector3 fixedBaseDir = getBreathFlatDir();
        Vector3 currentFillDir = fixedBaseDir;

        if (baseWarning != null)
        {
            baseWarning.playSector(fixedWarningPos, fixedBaseDir, breathDistance, breathTotalAngle, attackSpeed, breathBaseColor);
        }

        if (fillWwarning != null)
        {
            fillWwarning.playSector(fixedWarningPos, currentFillDir, breathDistance, breathAngle * 2f, attackSpeed, breathFillColor);
        }

        setBreathFxActive(false);

        yield return new WaitForSeconds(attackSpeed);

        if (ani != null) ani.SetTrigger("Attack02");

        float startDelay = 1f;
        while (startDelay > 0f)
        {
            currentFillDir = getBreathFlatDir();
            showBreathWarnings(baseWarning, fillWwarning, fixedWarningPos, fixedBaseDir, currentFillDir);

            startDelay -= Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            if (ani == null)
            {
                setBreathFxActive(false);
                hideBreathWarnings(baseWarning, fillWwarning);

                attackCoroutine = null;

                if (state != EnemyState.dead)
                {
                    state = EnemyState.chase;
                }

                yield break;
            }

            currentFillDir = getBreathFlatDir();
            showBreathWarnings(baseWarning, fillWwarning, fixedWarningPos, fixedBaseDir, currentFillDir);

            AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

            if (info.IsTag("Breath")) break;
            yield return null;
        }

        checkAttackTime();
        lastBreathHitTime = -Mathf.Infinity;
        setBreathFxActive(true);

        while (true)
        {
            if (ani == null) break;

            AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

            if (!info.IsTag("Breath")) break;

            currentFillDir = getBreathFlatDir();
            showBreathWarnings(baseWarning, fillWwarning, fixedWarningPos, fixedBaseDir, currentFillDir);

            syncBreathFxTransform();

            float time = info.normalizedTime;
            time = time - Mathf.Floor(time);

            bool canDamage = (time >= hitStart && time <= hitEnd);

            if (canDamage)
            {
                tryBreathDamage(fixedWarningPos, currentFillDir);
            }

            if (time > 0.95f)
            {
                setBreathFxActive(false);
                break;
            }

            yield return null;
        }

        setBreathFxActive(false);
        hideBreathWarnings(baseWarning, fillWwarning);

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
        }
    }

    private IEnumerator reflection_Co()
    {
        state = EnemyState.attack;
        isReflect = false;
        reflectResumeRequested = false;

        if (boxCol != null) boxCol.enabled = false;

        if (ani != null)
        {
            ani.speed = 1f;
            ani.SetTrigger("Attack03");
        }

        checkAttackTime();

        if (ani != null)
        {
            yield return null;

            while (true)
            {
                if (ani == null)
                {
                    if (boxCol != null) boxCol.enabled = false;
                    isReflect = false;
                    reflectResumeRequested = false;
                    attackCoroutine = null;

                    if (state != EnemyState.dead)
                    {
                        state = EnemyState.chase;
                    }
                    yield break;
                }

                AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

                if (info.IsTag("Reflect")) break;
                yield return null;
            }

            while (true)
            {
                if (ani == null)
                {
                    isReflect = false;
                    reflectResumeRequested = false;
                    attackCoroutine = null;

                    if (state != EnemyState.dead)
                    {
                        state = EnemyState.chase;
                    }
                    yield break;
                }

                AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

                if (!info.IsTag("Reflect")) break;

                float time = info.normalizedTime;
                time = time - Mathf.Floor(time);

                if (time >= stopRate) break;
                yield return null;
            }

            isReflect = true;

            if (boxCol != null) boxCol.enabled = true;

            ani.speed = 0f;

            float elapsed = 0f;

            while (elapsed < reflectionTime)
            {
                if (reflectResumeRequested) break;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (boxCol != null) boxCol.enabled = false;

            ani.speed = 1f;
            isReflect = false;
            reflectResumeRequested = false;

            while (true)
            {
                if (ani == null) break;

                AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

                if (!info.IsTag("Reflect")) break;
                yield return null;
            }
        }
        else
        {
            isReflect = true;

            if (boxCol != null) boxCol.enabled = true;

            yield return new WaitForSeconds(reflectionTime);

            if (boxCol != null) boxCol.enabled = false;

            isReflect = false;
            reflectResumeRequested = false;
        }

        if (boxCol != null) boxCol.enabled = false;

        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
        }
    }

    private IEnumerator rangeAttack_Co()
    {
        state = EnemyState.attack;
        isFlying = true;

        yield return new WaitForSeconds(rangeAttackSpeed);

        if (ani != null) ani.SetTrigger("Attack04");

        checkAttackTime();

        if (rangeAttackCount - rockPool.Count > 0)
        {
            for (int i = 0; i < rangeAttackCount - rockPool.Count; i++)
            {
                DragonProjectile rock = Instantiate(projectilePrefab, PoolsPos.transform);
                rock.dragon = this;
                rock.transform.localPosition = Vector3.zero;
                rock.gameObject.SetActive(false);
                rockPool.Enqueue(rock);
            }
        }

        for (int i = 0; i < rangeAttackCount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            Vector3 targetPos = player.transform.position + randomPos + Vector3.up * 10f;

            DragonProjectile rock = rockPool.Dequeue();
            WarningGizmo warning = getWarning();

            rock.transform.position = targetPos;
            rock.setWarning(warning);
            rock.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);
        }

        isFlying = false;
        attackCoroutine = null;

        if (state != EnemyState.dead)
        {
            state = EnemyState.chase;
        }
    }

    public void returnRock(DragonProjectile rock)
    {
        rock.transform.localPosition = Vector3.zero;
        rock.gameObject.SetActive(false);
        rockPool.Enqueue(rock);
    }

    public void spawnFireArea(Vector3 spawnPos)
    {
        if (rangeAttackCount - areaPool.Count > 0)
        {
            DragonFireArea temp = Instantiate(fireAreaPrefab, PoolsPos.transform);
            temp.dragon = this;
            temp.transform.localPosition = Vector3.zero;
            temp.gameObject.SetActive(false);
            areaPool.Enqueue(temp);
        }

        DragonFireArea area = areaPool.Dequeue();
        area.transform.position = spawnPos;
        area.gameObject.SetActive(true);
    }

    public void returnArea(DragonFireArea area)
    {
        area.transform.localPosition = Vector3.zero;
        area.gameObject.SetActive(false);
        areaPool.Enqueue(area);
    }

    private WarningGizmo getWarning()
    {
        if (warningPool.Count == 0) return null;

        WarningGizmo warning = warningPool.Dequeue();
        warning.init(returnWarning);
        return warning;
    }

    private void returnWarning(WarningGizmo warning)
    {
        if (state == EnemyState.dead) return;

        warning.transform.localPosition = Vector3.zero;
        warningPool.Enqueue(warning);
    }

    protected override void OnTriggerEnter(Collider other)
    { }

    public override void Move()
    { }

    protected override IEnumerator knockback_Co(Vector3 dir, float power)
    {
        state = EnemyState.chase;
        yield return null;
    }

    protected override void setPlayerPos()
    { }

    protected override void setMoveSpeed()
    { }

    protected override void TurnOffNavmesh()
    { }

    protected override void TurnOnNavmesh()
    { }

    protected override bool isItOnTheGround()
    {
        return true;
    }
}