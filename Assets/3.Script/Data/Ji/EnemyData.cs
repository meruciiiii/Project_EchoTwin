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
    public float detectRange;
    public float attackRange;
    public float coolTime;

    [Header("Drop")]
    public int dropEXP;
    public int dropGold;

    [Header("Prefab")]
    public GameObject prefab;

}
