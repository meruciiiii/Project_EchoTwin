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

    private WarningGizmo warning;
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.4f);

    private float startY = 0f;
    private float groundY = 0f;

    private void OnEnable()
    {
        velocityY = 0f;

        startY = transform.position.y;
        groundY = transform.position.y;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, ground))
        {
            groundY = hit.point.y;

            if (warning != null)
            {
                warning.showCircle(hit.point, attackRadius, warningColor);
                warning.setRatio(0f);
            }
        }
        else
        {
            if (warning != null)
            {
                Vector3 warningPos = transform.position;
                warningPos.y = 0f;

                warning.showCircle(warningPos, attackRadius, warningColor);
                warning.setRatio(0f);
            }
        }
    }

    private void Update()
    {
        velocityY -= gravityForce * Time.deltaTime;

        Vector3 move = Vector3.up * velocityY * Time.deltaTime;
        transform.position += move;

        updateWarningRatio();
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
                target.takeDamage(1, hit.point, 1);
                break;
            }

            if (warning != null)
            {
                warning.setRatio(1f);
                warning.Hide();
                warning = null;
            }

            sentinel.returnRock(this);
            return;
        }

        if (transform.position.y <= 0)
        {
            if (warning != null)
            {
                warning.Hide();
                warning = null;
            }

            sentinel.returnRock(this);
            return;
        }
    }

    public void setWarning(WarningGizmo warning)
    {
        this.warning = warning;
    }

    private void updateWarningRatio()
    {
        if (warning == null) return;

        float totalHeight = Mathf.Max(0.01f, startY - groundY);
        float currentHeight = Mathf.Clamp(transform.position.y - groundY, 0f, totalHeight);

        float progress = 1f - (currentHeight / totalHeight);

        warning.setRatio(progress);
    }
}
