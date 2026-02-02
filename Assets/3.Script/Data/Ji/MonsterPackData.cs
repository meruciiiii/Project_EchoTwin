using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterEntry
{
    public int monsterID;
    public int count;
}
public class MonsterPackData : ScriptableObject
{
    public int monsterPackID;
    public List<MonsterEntry> monsters;
    public int totalCount;
    public string description;
}
