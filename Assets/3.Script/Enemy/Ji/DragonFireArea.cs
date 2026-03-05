using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireArea : MonoBehaviour
{
    public RedDragon dragon;

    private Coroutine coroutine;
    private Coroutine existCoroutine;

    [SerializeField] private float existTime = 3f;
    [SerializeField] private float timePerDMG = 1f;
    [SerializeField] private float minTimeForDMG = 0.5f;

    private PlayerAction target;

    private void OnEnable()
    {
        target = null;
        coroutine = null;

        if(existCoroutine != null) StopCoroutine(existCoroutine);
        existCoroutine = StartCoroutine(exist_Co());
    }

    private void OnDisable()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        if(existCoroutine != null)
        {
            StopCoroutine(existCoroutine);
            existCoroutine = null;
        }

        target = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerAction player = other.GetComponentInParent<PlayerAction>();
        if (player == null) return;
        if (coroutine != null) return;

        target = player;
        coroutine = StartCoroutine(tikDMG_Co());
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerAction player = other.GetComponentInParent<PlayerAction>();
        if (player == null) return;
        if (player != target) return;

        target = null;
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator tikDMG_Co()
    {
        yield return new WaitForSeconds(minTimeForDMG);

        while (target != null)
        {
            target.takeDamage(1, transform.position, 0);
            yield return new WaitForSeconds(timePerDMG);
        }

        coroutine = null;
    }

    private IEnumerator exist_Co()
    {
        yield return new WaitForSeconds(existTime);

        existCoroutine = null;
        dragon.returnArea(this);
    }
}
