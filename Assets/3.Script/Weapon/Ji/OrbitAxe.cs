using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAxe : MonoBehaviour
{
    private Transform player;
    private float angle;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float radius = 2f;

    public void Init(Transform player)
    {
        this.player = player;
        angle = 0f;
    }

    private void Update()
    {
        angle += Time.deltaTime * speed;

        transform.position = player.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

        if(angle >= Mathf.PI * 2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            //other.getcomponent<enemy>().takedamage(damage);
        }
    }
}
