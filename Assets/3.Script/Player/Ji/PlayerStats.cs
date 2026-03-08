using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 6; //��ĭ�� ü�� 1�� ������ ����
    private int currentHP = 0;
    public bool isDead => (currentHP <= 0);
    [SerializeField] private float playerDMG = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackSpeed = 1f;

    [Header("Dash Info")]
    [SerializeField] private float dashLength = 1f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashDelay = 1f;
    public bool isDash = false;

    [Header("Take Damage")]
    [SerializeField] private int flashAmount = 3;//�����̴� Ƚ��
    [SerializeField] private float flashDuration = 0.1f;//1ȸ ������ �� �ɸ��� �ð�

    [Header("Delay to Echo")]
    [SerializeField] private float timeBetweenAttack = 0.5f;

    [Header("Player Physics")]
    [SerializeField] public float invincibilityTime = 1f;
    [SerializeField] private float knockBackForce = 2f;


    [Header("Player Currency")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int cristal = 0;

    [Header("Upgrade Stats")]
    [SerializeField] private float echoDamage = 0f;
    [SerializeField] private float attackRange = 0f;

    public event Action<int, int> onHpChanged;
    public event Action<int> onMaxHpChanged;

    public event Action<int> onCoinChanged;
    public event Action<int> onCristalChanged;

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
    public float KnockBackForce => knockBackForce;
    public int Gold => gold;
    public int Cristal => cristal;
    public float EchoDamage => echoDamage; 
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;

    private void Awake()
    {
        //currentHP = GameManager.instance.currentHP;
        //maxHP = GameManager.instance.maxHP;
        //gold = GameManager.instance.playerGold;
        //cristal = GameManager.instance.playerCristal;
        if (currentHP == 0)
        {
            currentHP = maxHP;
        }
    }

    private void Start()
    {
        onMaxHpChanged?.Invoke(maxHP);
        onHpChanged?.Invoke(currentHP, maxHP);
        onCoinChanged?.Invoke(gold);
        onCristalChanged?.Invoke(cristal);
    }

    private void OnEnable()
    {

    }

    public void setCurrentHP()
    {
        if(currentHP == 0)
        {
            currentHP = maxHP;
        }
        onHpChanged?.Invoke(currentHP, maxHP);
    }

    public void takeDamage(int damage)
    {
        if (isDash) return;

        currentHP -= 1;
        onHpChanged?.Invoke(currentHP, maxHP);
    }

    public void resetGold()
    {
        gold = 0;

        onCoinChanged?.Invoke(gold);
    }

    public void getGold(int amount)
    {
        gold += amount;
        onCoinChanged?.Invoke(gold);

        //Debug.Log($"{gold} gold");
    }

    public bool TryUseGold(int amount)
    {
        if (gold < amount) return false;

        gold -= amount;
        onCoinChanged?.Invoke(gold);
        return true;
    }

    public void getCristal(int amount)
    {
        cristal += amount;
        onCristalChanged?.Invoke(cristal);
    }

    public bool TryUseCristal(int amount)
    {
        if (cristal < amount) return false;

        cristal -= amount;
        onCristalChanged?.Invoke(cristal);
        return true;
    }

    public void getHeart(int value)
    {
        if (currentHP >= maxHP) return;
        if (value <= 0) return;

        currentHP = Mathf.Min(currentHP + value, maxHP);
        onHpChanged?.Invoke(currentHP, maxHP);
    }
    public void getMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        onMaxHpChanged?.Invoke(maxHP);
        onHpChanged?.Invoke(currentHP, maxHP);
    }

    public void getPlayerDMG(float amount)
    {
        playerDMG += amount;
    }

    public void getMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void getEchoDamage(float amount)
    {
        echoDamage += amount;
    }

    public void getAttackRange(float amount)
    {
        attackRange += amount;
    }

    public void getAttackSpeed(float amount)
    {
        attackSpeed += amount;
    }
    public void Developer()
    {
        currentHP = 10000;
        playerDMG = 10000;
        cristal = 10000;
    }

}
