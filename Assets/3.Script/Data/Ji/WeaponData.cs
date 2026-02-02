using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("ID")]
    public int ID;
    public string name;

    [Header("Stats")]
    public float baseDamage;
    public float attackSpeed;
    public float attackRange;
    public float knockback;

    [Header("Echo")]
    public float echoDMGRatio;
    public string echoDescription;

    [Header("Image")]
    public Sprite icon;

    [Header("Combo")]
    public int comboCount;
    public float comboCooltime;

    public string description;
}
