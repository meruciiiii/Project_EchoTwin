using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerProjectile : MonoBehaviour
{
    private EnemyStateAbstract enemy;
    private bool hasHit;

    private Collider col;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyStateAbstract>();
        gameObject.SetActive(false);
        TryGetComponent<Collider>(out col);
    }

    private void OnEnable()
    {
        hasHit = false;
        col.enabled = false;
        StartCoroutine(Attack_Co());
    }

    private void OnDisable()
    {
        hasHit = false;
        col.enabled = false;
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            other.GetComponent<PlayerAction>().takeDamage((int)enemy.Damage, transform.position);
        }
    }

    private IEnumerator Attack_Co()
    {
        yield return new WaitForSeconds(0.5f);

        col.enabled = true;

        yield return new WaitForSeconds(1.2f);

        col.enabled = false;
    }
}
