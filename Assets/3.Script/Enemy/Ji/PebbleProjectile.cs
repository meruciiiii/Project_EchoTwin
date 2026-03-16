using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PebbleProjectile : MonoBehaviour
{
    private EnemyStateAbstract enemy;
    private bool hasHit;
    private Collider col;
    private ParticleSystem particleSystem;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyStateAbstract>();
        particleSystem = GetComponent<ParticleSystem>();

        gameObject.SetActive(false);
        TryGetComponent<Collider>(out col);
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        hasHit = false;
        gameObject.SetActive(true);

        if(particleSystem != null)
        {
            particleSystem.Play(true);
        }
    }

    private void OnDisable()
    {
        if(particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            other.GetComponent<PlayerAction>().takeDamage((int)enemy.Damage, transform.position,1);
            gameObject.SetActive(false);
        }
    }
}
