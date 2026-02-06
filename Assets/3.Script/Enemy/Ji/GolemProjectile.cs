using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemProjectile : MonoBehaviour
{
    private EnemyStateAbstract enemy;
    private bool hasHit;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyStateAbstract>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            other.GetComponent<PlayerStats>().takeDamage(enemy.GetComponent<EnemyData>().damage);
            gameObject.SetActive(false);
        }
    }
}
