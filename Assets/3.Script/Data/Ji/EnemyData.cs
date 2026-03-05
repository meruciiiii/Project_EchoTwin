using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    [Header("ID")]
    public int id;
    public string name;
    public string tier;

    [Header("Stats")]
    public int maxHP;
    public int damage;
    public float moveSpeed;
    public float attackSpeed;
    public float attackRange;
    public float coolTime;

    [Header("Drop")]
    public int dropGold;
    public int minCristal;
    public int maxCristal;
    public int minWeight;
    public int maxWeight;

    [Header("Prefab")]
    public GameObject prefab;

}
