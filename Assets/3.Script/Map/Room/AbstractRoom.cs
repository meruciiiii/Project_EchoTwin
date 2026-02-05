
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRoom : MonoBehaviour
{
    private GameObject[] roomList;
    public void GetRoomData()
    {
        MapCreater mapCreater;
    }
    public void GetRoomFromTag(int roomNum)        // Tag를 확인해서 Room 번호 및 종류에 따라 맵을 찾는
    {
        string roomName = "room" + roomNum;
        roomList = GameObject.FindGameObjectsWithTag(roomName);
    }
    public void GetPosition()        // Tag로 찾은 맵의 spawnPosition 값들을 찾아오는
    {

    }
    public void SetPlayerSpawnPosition()        // player의 spawnPosition 중 현재 활용할 위치 설정
    {

    }
    public void SetEnemySpawn()        // 맵에 따라서 정해진 적을 불러오고 설정하는
    {

    }
}