using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : EnemyStateAbstract
{
    [Header("RangeAttack")]
    [SerializeField] private float rangeAttackSpeed = 1f;
    [SerializeField] private float rangeAttackRange = 1f;
    [SerializeField] private int rangeAttackCount = 1;
    [SerializeField] GameObject projectilePrefab;
    private Queue<GameObject> rockPoll;

    [Header("Spawn")]
    [SerializeField] GameObject rangeMobPrefab;
    [SerializeField] private int rangeCount = 0;
    private Queue<GameObject> rangeMobPool;
    [SerializeField] GameObject meleeMobPrefab;
    [SerializeField] private int meleeCount = 0;
    private Queue<GameObject> meleeMobPool;

    [Header("2ndPhaseStart")]
    [Range(0f, 1f)]
    [SerializeField] private float phaseStartHP = 0.4f;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < rangeAttackCount * 2; i++)
        {
            GameObject rock = Instantiate(projectilePrefab);
            rock.transform.localPosition = Vector3.zero;
            rock.SetActive(false);
            rockPoll.Enqueue(rock);
        }
        for (int i = 0; i < rangeCount; i++)
        {
            GameObject rangeMob = Instantiate(rangeMobPrefab, transform);
            rangeMob.transform.localPosition = Vector3.zero;
            rangeMob.SetActive(false);
            rangeMobPool.Enqueue(rangeMob);
        }
        for (int i = 0; i < meleeCount; i++)
        {
            GameObject meleeMob = Instantiate(meleeMobPrefab, transform);
            meleeMob.transform.localPosition = Vector3.zero;
            meleeMob.SetActive(false);
            meleeMobPool.Enqueue(meleeMob);
        }
    }

    public override void Attack()
    {
        if (state == EnemyState.attack) return;
        if (coroutine != null) return;
        if (!canAttack())
        {
            Move();
            return;
        }

        Vector3 targetPos = player.transform.position;
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(targetPos, startPos);

        if (distance > enemyData.attackRange - 0.5f)
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(RangeAttack_Co());
        }
        else
        {
            if (coroutine != null) return;
            int temp = Random.Range(0, 2);
            if (temp == 0)
            {
                coroutine = StartCoroutine(Attack_Co());
            }
            if (temp == 1)
            {
                coroutine = StartCoroutine(MobSpawn_Co());
            }
        }
    }

    private IEnumerator Attack_Co()
    {

        yield return null;
    }

    private IEnumerator RangeAttack_Co()
    {
        int temp = 0;
        while (rangeAttackCount > temp)
        {
            Vector3 randomPos = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
            Vector3 targetPos = player.transform.position + randomPos + Vector3.up * 10f;

            GameObject rock = rockPoll.Dequeue();
            rock.SetActive(true);
            rock.transform.localPosition = targetPos - transform.position;

            temp++;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator MobSpawn_Co()
    {
        int temp = Random.Range(0, 2);
        if(temp == 0)
        {

        }
        if(temp == 1)
        {

        }
        yield return null;
    }

    public override void Move()
    {
        if (state != EnemyState.chase) return;
        if (coroutine != null) return;

        setPlayerPos();
    }
}
