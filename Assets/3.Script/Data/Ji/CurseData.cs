using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseData : ScriptableObject
{
    [System.Serializable]
    public class CurseStat
    {
        public float damage;
        public float coolTime;
    }

    [CreateAssetMenu(fileName = "New Curse Data", menuName = "Equipment/Curse Data")]
    public class WeaponData : ScriptableObject
    {
        public MeshRenderer image;
        public Sprite icon;
        public string name;
        public CurseStat[] stats;

        public CurseStat getStat(int level)
        {
            int index = Mathf.Clamp(level - 1, 0, stats.Length);
            return stats[index];
        }
    }
}
