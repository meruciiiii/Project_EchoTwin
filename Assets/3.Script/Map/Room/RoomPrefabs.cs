
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
    private void Awake()
    {
        physicalStartRoom = GameObject.FindGameObjectWithTag("R_101");
        physicalBattleRoom[0] = GameObject.FindGameObjectWithTag("R_01");
        physicalBattleRoom[1] = GameObject.FindGameObjectWithTag("R_02");
        physicalBattleRoom[2] = GameObject.FindGameObjectWithTag("R_03");
        physicalBattleRoom[3] = GameObject.FindGameObjectWithTag("R_04");
        physicalBattleRoom[4] = GameObject.FindGameObjectWithTag("R_05");
        physicalBattleRoom[5] = GameObject.FindGameObjectWithTag("R_06");
        physicalBattleRoom[6] = GameObject.FindGameObjectWithTag("R_07");
        physicalBattleRoom[7] = GameObject.FindGameObjectWithTag("R_08");
        physicalBattleRoom[8] = GameObject.FindGameObjectWithTag("R_09");
        physicalBattleRoom[9] = GameObject.FindGameObjectWithTag("R_10");
        physicalBattleRoom[10] = GameObject.FindGameObjectWithTag("R_11");
        physicalBattleRoom[11] = GameObject.FindGameObjectWithTag("R_12");
        physicalBattleRoom[12] = GameObject.FindGameObjectWithTag("R_13");
        physicalBattleRoom[13] = GameObject.FindGameObjectWithTag("R_14");
        physicalBattleRoom[14] = GameObject.FindGameObjectWithTag("R_15");
        physicalBattleRoom[15] = GameObject.FindGameObjectWithTag("R_16");
        physicalBattleRoom[16] = GameObject.FindGameObjectWithTag("R_17");
        physicalBattleRoom[17] = GameObject.FindGameObjectWithTag("R_18");
        physicalShopRoom = GameObject.FindGameObjectWithTag("R_102");
        physicalForgeRoom = GameObject.FindGameObjectWithTag("R_103");
        physicalEliteRoom[0] = GameObject.FindGameObjectWithTag("R_19");
        physicalEliteRoom[1] = GameObject.FindGameObjectWithTag("R_20");
        physicalRewardRoom = GameObject.FindGameObjectWithTag("R_104");
        physicalFirstBossRoom = GameObject.FindGameObjectWithTag("R_21");
        physicalSecondBossRoom = GameObject.FindGameObjectWithTag("R_22");
    }
}
//Start, Battle, Shop, Forge, Elite, Reward, Boss, count