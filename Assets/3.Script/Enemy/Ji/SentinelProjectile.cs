using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelProjectile : MonoBehaviour
{
    //-y 방향으로 힘을 줘서 가속도 만들고
    //바닥에 닿으면 particle 이랑 collider 검사해서 player damage
    public Sentinel sentinel;

    [SerializeField] private float gravityForce = 5f;
    [SerializeField] private float rayDistance = 0.5f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask player;
    private float velocityY = 0f;

    [Space(3f)]
    [SerializeField] private float attackRadius = 3f;

    private void OnEnable()
    {
        velocityY = 0f;
    }

    private void Update()
    {
        velocityY -= gravityForce * Time.deltaTime;

        Vector3 move = Vector3.up * velocityY * Time.deltaTime;
        transform.position += move;

        checkOnGround(move.y);
    }

    private void checkOnGround(float moveY)
    {
        float distance = Mathf.Abs(moveY) + rayDistance;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distance, ground))
        {
            Collider[] hits = Physics.OverlapSphere(hit.point, attackRadius, player);
            foreach (Collider col in hits)
            {
                PlayerAction target = col.GetComponentInParent<PlayerAction>();
                if (target == null) continue;

                target.takeDamage(1, sentinel.transform.position);
                break;
            }

            sentinel.returnRock(this);
        }
    }
}
