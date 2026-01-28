using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 6; //반칸이 체력 1로 기준을 설정
    [SerializeField] private float playerDMG = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashLength = 1f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashDelay = 1f;

    public int MaxHP => maxHP;
    public float PlayerDMG => playerDMG;
    public float MoveSpeed => moveSpeed;
    public float DashLength => dashLength;
    public float DashSpeed => dashSpeed;
    public float DashDelay => dashDelay;
}
