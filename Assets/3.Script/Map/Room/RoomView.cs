using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomView : MonoBehaviour
{
    public event Action<Vector2Int> OnDoorUsed;
    private DoorTrigger[] doors;
    //bridge
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
                door.Lock(0.5f);
                return door.gameObject.transform.position + new Vector3(direction.x * 2.2f, 0f, direction.y * 2.2f);
            }
        }
        Debug.Log("Can't find door");
        return Vector3.zero;
    }
    public void DoorAccordingState(FloorData floor)
    {
        //Debug.Log("DoorAccordingState Setting is //문이 열려있고 아래에 있으면 위로, 문이 닫혀있고 위에 있으면 아래로 - 초기 세팅 Start");
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            BridgeMoving bridgeMoving;
            if (!door.TryGetComponent(out bridgeMoving))
                Debug.Log("TryGetComponent BridgeMoving is fail");
            //if (GameManager.instance.monsterCount > 0)
            //if (!GameManager.instance.isGetWeapon)
                bridgeMoving.EnterDoor(floor.GetDoorState(door.doorIntDirection));
        }
    }//문이 열려있고 아래에 있으면 위로, 문이 닫혀있고 위에 있으면 아래로 - 초기 세팅
    public void EnterStartRoomFirst(FloorData floor)
    {
        //Debug.Log("EnterStartRoomFirst Setting is //시작 방 모두 내려버리기 Start");
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            BridgeMoving bridgeMoving;
            if (!door.TryGetComponent(out bridgeMoving))
                Debug.Log("TryGetComponent BridgeMoving is fail");
            bridgeMoving.SetStartRoom();
        }
    }//시작 방 모두 내려버리기
    public void DoorResetting()
    {
        //Debug.Log("DoorResetting Setting is //위에있으면 리턴, 아래에 있으면 업 - 전부 위로 올리기 위함 - 방 나갈 때 Start");
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            BridgeMoving bridgeMoving;
            if (!door.TryGetComponent(out bridgeMoving))
                Debug.Log("TryGetComponent BridgeMoving is fail");
            bridgeMoving.ResetBridge();
        }
    }//위에있으면 리턴, 아래에 있으면 업          - 전부 위로 올리기 위함 - 방 나갈 때
    public void BridgeisMove(FloorData floor)
    {
        //Debug.Log("BridgeisMove Setting is //열린 문 일것 - 위에 있으면 아래로, 아래면 위로 - 상태 전환을 위함 - 도착 했을 때, 무기 들었을 때 Start");
        foreach (DoorTrigger door in doors)
        {
            if (door == null)
            {
                Debug.Log("DoorTrigger Setting is failed");
                continue;
            }
            BridgeMoving bridgeMoving;
            if (!door.TryGetComponent(out bridgeMoving))
                Debug.Log("TryGetComponent BridgeMoving is fail");
            bridgeMoving.SetState(floor.GetDoorState(door.doorIntDirection));
        }
    }//열린 문 일것 - 위에 있으면 아래로, 아래면 위로 - 상태 전환을 위함 - 도착 했을 때, 무기 들었을 때
}
