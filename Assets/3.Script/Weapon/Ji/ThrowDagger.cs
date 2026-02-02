using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDagger : MonoBehaviour
{
    private Transform target;
    private float damage;
    [SerializeField] private float speed = 10f;

    private void Update()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if(Vector3.Distance(transform.position,target.position)<0.1f)
        {
            HitTarget();
        }
    }

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void HitTarget()
    {
        //target.getcomponent<enemy>().takeDamage(damage); ¥¿≥¶¿∏∑Œ µ•πÃ¡ˆ∏¶ ¡‹
        target.GetComponent<EnemyStateAbstract>().takeDamage(damage);
        Destroy(gameObject);
    }
}
