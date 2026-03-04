using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDagger : MonoBehaviour
{
    private Transform target;
    private float damage;
    [SerializeField] private float speed = 15f;

    private void Update()
    {
        Vector3 targetPosFlat = new Vector3(target.position.x, transform.position.y, target.position.z);

        Vector3 dir = (targetPosFlat - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);
        }

        if (Vector3.Distance(transform.position, targetPosFlat) < 0.1f)
        {
            HitTarget();
        }
    }

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
        SoundManager.SendEvent(SoundType.SFX_DaggerThrow);
    }

    private void HitTarget()
    {
        //target.getcomponent<enemy>().takeDamage(damage); ¥¿≥¶¿∏∑Œ µ•πÃ¡ˆ∏¶ ¡‹
        target.GetComponent<EnemyStateAbstract>().takeDamage(damage);
        Destroy(gameObject);
    }
}
