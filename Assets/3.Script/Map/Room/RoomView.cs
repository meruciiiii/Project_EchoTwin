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

            if (!floor.GetRoomData().GetRoomType().Equals(RoomType.Start))
                bridgeMoving.EnterDoor(floor.GetDoorState(door.doorIntDirection));
            else
            {
                bridgeMoving.SetStartRoom();
            }
            //bridgeMoving.SetState(floor.GetDoorState(door.doorIntDirection));
        }
    }
    public void DoorResetting()
    {
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
    }
    public void BridgeisMove(FloorData floor)
    {
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
    }
}
