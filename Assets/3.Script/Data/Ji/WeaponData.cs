using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponStat
{
    public float damage;
    public float speed;
}

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Equipment/Weapon Data")]
public class WeaponData : ScriptableObject
{
    //public int id; 무기 고유번호 0:검 1: 도끼...
    public MeshRenderer image;
    public Sprite icon;
    public string name;
    public WeaponStat[] stats;

    public WeaponStat getStat(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, stats.Length);
        return stats[index];
    }
}
