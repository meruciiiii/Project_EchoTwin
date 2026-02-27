using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 6; //ÇÑÄ­ÀÌ Ã¼·Â 1·Î ±âÁØÀ» ¼³Á¤
    private int currentHP = 0;
    public bool isDead => (currentHP <= 0);
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

    [Header("Player Physics")]
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private float knockBackForce = 2f;


    [Header("Player Currency")]
    [SerializeField] private int gold = 1000;
    [SerializeField] private int cristal = 0;

    public event Action<int, int> onHpChanged;
    public event Action<int> onMaxHpChanged;

    public event Action<int> onCoinChanged;

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
    public float InvincibilityTime => invincibilityTime;
    public float KnockBackForce => knockBackForce;
    public int Gold => gold;
    public int Cristal => cristal;

    private void Awake()
    {
        //currentHP = GameManager.instance.currentHP;
        //maxHP = GameManager.instance.maxHP;
        //gold = GameManager.instance.playerGold;
        //cristal = GameManager.instance.playerCristal;
    }

    private void Start()
    {
        setHP();
        setGold();
    }

    private void setHP()
    {
        if (currentHP != 0)
        {
            onMaxHpChanged?.Invoke(maxHP);
            onHpChanged?.Invoke(currentHP, maxHP);
            return;
        }
        else
        {
            currentHP = maxHP;
            onMaxHpChanged?.Invoke(maxHP);
            onHpChanged?.Invoke(currentHP, maxHP);
        }
    }

    private void setGold()
    {
        onCoinChanged?.Invoke(gold);
    }

    public void takeDamage(int damage)
    {
        if (isDash) return;

        currentHP -= 1;
        onHpChanged?.Invoke(currentHP, maxHP);
    }

    public void getGold(int amount)
    {
        gold += amount;
        onCoinChanged?.Invoke(gold);

        Debug.Log($"{gold} gold");
    }

    public void getCristal(int amount)
    {
        cristal += amount;
        Debug.Log($"{cristal} cristal");
    }

    public void getHeart()
    {
        currentHP += 1;
        onHpChanged?.Invoke(currentHP, maxHP);

        Debug.Log($"{currentHP} after HP");
    }

    public void getMaxHP()
    {
        maxHP += 1;
        onMaxHpChanged?.Invoke(maxHP);
    }
}
