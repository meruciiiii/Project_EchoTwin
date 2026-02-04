using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 6; //¹ÝÄ­ÀÌ Ã¼·Â 1·Î ±âÁØÀ» ¼³Á¤
    private int currentHP;
    public bool isDead => currentHP <= 0;
    [SerializeField] private float playerDMG = 1f;
    [SerializeField] private float moveSpeed = 1f;

    [Header("Dash Info")]
    [SerializeField] private float dashLength = 1f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashDelay = 1f;
    public bool isDash = false;

    [Header("Take Damage")]
    [SerializeField] private int flashAmount = 3;//±ôºýÀÌ´Â È½¼ö
    [SerializeField] private float flashDuration = 0.1f;//1È¸ ±ôºýÀÏ ¶§ °É¸®´Â ½Ã°£

    [Header("Delay to Echo")]
    [SerializeField] private float timeBetweenAttack = 0.5f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public float PlayerDMG => playerDMG;
    public float MoveSpeed => moveSpeed;
    public float DashLength => dashLength;
    public float DashSpeed => dashSpeed;
    public float DashDelay => dashDelay;
    public int FlashAmount => flashAmount;
    public float FlashDuration => flashDuration;
    public float TimeBetweenAttack => timeBetweenAttack;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void takeDamage(int damage)
    {
        if (isDash) return;
        currentHP -= damage;
    }
}
