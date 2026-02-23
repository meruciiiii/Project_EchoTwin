using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Map/monsterPacks")]
public class MonsterPackList : ScriptableObject
{
    public MonsterPack[] monsterPacks = new MonsterPack[11];
#if UNITY_EDITOR
    [ContextMenu("MonsterPack Create")]
    void CreatePacks()
    {
        for (int i = 0; i < monsterPacks.Length; i++)
        {
            monsterPacks[i] = new MonsterPack();
            monsterPacks[i].monsterPackID = i+1;
            monsterPacks[i].monsterDataList = new List<MonsterData>();
        }
    }
#endif
}
[System.Serializable]
public class MonsterPack
{
    public int monsterPackID;
    public List<MonsterData> monsterDataList;
}

[System.Serializable]
public class MonsterData
{
    public GameObject monsterPrefab;
    public int count;
}