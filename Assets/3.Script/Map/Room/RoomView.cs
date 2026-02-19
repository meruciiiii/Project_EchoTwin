using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomView : MonoBehaviour
{
    public event Action<Vector2Int> OnDoorUsed;
    private DoorTrigger[] doors;
    //bridge
    private BridgeMoving bridgeMoving;
    private void Awake()
    {
        if (!TryGetComponent(out bridgeMoving))
            Debug.Log("TryGetComponent BridgeMoving is fail");
    }
    public void SetDoors(GameObject roomPrefab)
    {
        if (doors != null)
        {
            foreach (DoorTrigger door in doors)
            {
                if (door == null) continue;
                door.onPlayerEnter -= HandleDoorEnter;
            }
        }
        doors = roomPrefab.GetComponentsInChildren<DoorTrigger>(true);
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            door.onPlayerEnter += HandleDoorEnter;
        }
    }
    private void HandleDoorEnter(Vector2Int direction)
    {
        OnDoorUsed?.Invoke(direction);
    }
    public Vector3 GetDoor(Vector2Int direction)
    {
        foreach (DoorTrigger door in doors)
        {
            if (door.Direction.Equals(-direction))
            {
                return door.gameObject.transform.position;
            }
        }
        Debug.Log("Can't find door");
        return Vector3.zero;
    }
    public void DoorAccordingState(FloorData floor)
    {
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            bridgeMoving.StartMoving(door.gameObject, floor.GetDoorState(door.doorIntDirection));
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
    //public void Initialize(Room logicalRoom, Action<Vector2Int> onDoorEnter)
    //{
    //}
    //public Transform[] GetMonsterSpawnPoints()
}
