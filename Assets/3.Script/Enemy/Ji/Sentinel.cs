using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : EnemyStateAbstract
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 0f;

    [Header("Attack")]
    [SerializeField] private float normalAttackSpeed = 0f;
    [SerializeField] private float normalAttackRange = 0f;

    [Space(5f)]
    [SerializeField] private float areaAttackSpeed = 0f;
    [SerializeField] private float areaAttackRange = 0f;
    [SerializeField] private int areaAttackCount = 0;

    [Header("Spawn")]
    [SerializeField] GameObject rangeMobPrefab;
    [SerializeField] private int rangeCount = 0;
    [SerializeField] GameObject meleeMobPrefab;
    [SerializeField] private int meleeCount = 0;

    [Header("2ndPhaseStart")]
    [Range(0f,1f)] 
    [SerializeField] private float phaseStartHP = 0.4f;

    public override void Attack()
    {

    }

    public override void Move()
    {

    }
}
