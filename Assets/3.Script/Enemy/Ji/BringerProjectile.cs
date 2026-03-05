using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerProjectile : MonoBehaviour
{
    private EnemyStateAbstract enemy;
    private bool hasHit;

    private Collider col;
    private Animator ani;

    [SerializeField]
    [Range(0f, 1f)] private float hitStart = 0.3f;
    [SerializeField]
    [Range(0f, 1f)] private float hitEnd = 0.8f;

    private Coroutine coroutine;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyStateAbstract>();
        TryGetComponent(out col);
        ani = GetComponentInChildren<Animator>();
        gameObject.SetActive(false);
        if (col != null)
        {
            col.enabled = false;
            col.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        hasHit = false;
        if (col != null) col.enabled = false;

        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Attack_Co());
    }

    private void OnDisable()
    {
        hasHit = false;
        if (col != null) col.enabled = false;

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator Attack_Co()
    {
        while(true)
        {
            if (ani == null) break;

            AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);

            float time = info.normalizedTime;
            time = time - Mathf.Floor(time);

            bool canDamage = (time >= hitStart && time <= hitEnd);
            if (col != null) col.enabled = canDamage;

            if(time > 0.99f)
            {
                col.enabled = false;
                gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            other.GetComponent<PlayerAction>().takeDamage((int)enemy.Damage, transform.position,1);
        }
    }
}
