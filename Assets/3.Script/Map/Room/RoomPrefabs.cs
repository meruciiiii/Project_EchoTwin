
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefabs : MonoBehaviour
{
    [SerializeField] private GameObject physicalStartRoom;
    [SerializeField] private List<GameObject> physicalBattleRoom;
    [SerializeField] private GameObject physicalShopRoom;
    [SerializeField] private GameObject physicalForgeRoom;
    [SerializeField] private GameObject physicalEliteRoom;
    [SerializeField] private GameObject physicalRewardRoom;
    [SerializeField] private GameObject physicalFirstBossRoom;
    [SerializeField] private GameObject physicalSecondBossRoom;
}
//Start, Battle, Shop, Forge, Elite, Reward, Boss, count