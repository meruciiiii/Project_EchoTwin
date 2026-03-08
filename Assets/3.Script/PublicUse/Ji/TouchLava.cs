using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TouchLava : MonoBehaviour
{
    [SerializeField] private float checkRadius = 10f;
    [SerializeField] private float yOffset = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerAction player;
            //if (!other.TryGetComponent<PlayerAction>(out player)) return;

            Vector3 destPos;
            if (takePlayerOnGround(other.transform.position, out destPos))
            {
                other.transform.position = destPos;

                Rigidbody rb = other.attachedRigidbody;
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }
            other.GetComponent<PlayerAction>().takeDamage(1, transform.position, 0);

            return;
        }

        if(other.CompareTag("Enemy"))
        {
            EnemyStateAbstract target;
            if(!other.TryGetComponent<EnemyStateAbstract>(out target)) return;

            target.takeDamage(99999);
        }
    }

    private bool takePlayerOnGround(Vector3 from, out Vector3 destPos)
    {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(from, out hit, checkRadius, NavMesh.AllAreas))
        {
            destPos = hit.position + Vector3.up * yOffset;
            return true;
        }

        destPos = default;
        return false;
    }
}
