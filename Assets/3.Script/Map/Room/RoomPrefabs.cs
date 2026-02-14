
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefabs : ScriptableObject
{
    [SerializeField] public GameObject physicalStartRoom;
    [SerializeField] public List<GameObject> physicalBattleRoom;
    [SerializeField] public GameObject physicalShopRoom;
    [SerializeField] public GameObject physicalForgeRoom;
    [SerializeField] public List<GameObject> physicalEliteRoom;
    [SerializeField] public GameObject physicalRewardRoom;
    [SerializeField] public GameObject physicalFirstBossRoom;
    [SerializeField] public GameObject physicalSecondBossRoom;
}
//Start, Battle, Shop, Forge, Elite, Reward, Boss, count