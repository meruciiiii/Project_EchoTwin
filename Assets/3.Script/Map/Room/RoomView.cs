
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomView : MonoBehaviour
{
    private DoorTrigger[] doors;
    private Room logicalRoom;
    private void Awake()
    {
        if(!TryGetComponent(out doors))
        {
            Debug.Log("TryGetComponent DoorTrigger is fail");
        }
    }
    //문 다리 열림/닫힘 시각 처리
    //몬스터 스폰 위치
    //플레이어 스폰 위치
    //이펙트, 애니메이션
    /*
        Logical Room의 문 상태 반영
        DoorTrigger 활성/비활성
        이벤트 구독
     */
    public void Initialize(Room logicalRoom, Action<Vector2Int> onDoorEnter)
    {

    }
    public Vector3 GetDoorSpawnPosition(Vector2Int direction)
    {
        return Vector3.zero;
    }
    //public Transform[] GetMonsterSpawnPoints()
}
