
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjects : MonoBehaviour
{
    [SerializeField] public GameObject physicalStartRoom;
    [SerializeField] public GameObject[] physicalBattleRoom = new GameObject[18];
    [SerializeField] public GameObject physicalShopRoom;
    [SerializeField] public GameObject physicalForgeRoom;
    [SerializeField] public GameObject[] physicalEliteRoom = new GameObject[2];
    [SerializeField] public GameObject physicalRewardRoom;
    [SerializeField] public GameObject physicalFirstBossRoom;
    [SerializeField] public GameObject physicalSecondBossRoom;
}
//Start, Battle, Shop, Forge, Elite, Reward, Boss, count