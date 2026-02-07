
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMoving : MonoBehaviour
{
    // 현재 룸에서 나가는 위치
    // 현재 룸과 연결된 룸 정보
    // 나가는 위치에 따른 연결된 룸에서 들어가야 할 위치
    // 플레이어는 강제로 안으로 들어간다.
    // (배틀룸 일 경우) 
    // 플레이어가 완전히 들어가면 다리를 사라지게(내려가도록) 설정
    // 플레이어가 몬스터를 다 잡으면 다리가 다시 올라온다
    private Vector2Int currentRoom;                                                 // 현재 플레이어가 있는 방의 룸 위치(관계적)
    private Vector2Int nextRoom;                                                    // 플레이어가 넘어갈 방의 룸 위치(관계적)
    private GameObject currentRoomObject;                                           // 현재 플레이어가 있는 방의 오브젝트
    private GameObject nextRoomObject;                                              // 플레이어가 넘어갈 방의 오브젝트
    private GameObject currentPlayerSPs;                                            // 현재 플레이어가 있는 방의 플레이어 스폰그룹의 상위 오브젝트
    private GameObject nextPlayerSPs;                                               // 플레이어가 넘어갈 방의 플레이어 스폰그룹의 상위 오브젝트
    private GameObject player;                                                      // 위치를 옮길 플레이어 오브젝트

    private void Awake()
    {
        
    }
    private void RoomCheck()
    {

    }
}
