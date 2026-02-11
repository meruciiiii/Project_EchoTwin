using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapCreater : MonoBehaviour
{
    //[SerializeField] public Dictionary<Vector2Int, MapData> microMap;          // map node is here
    //private MapData mapData;                                                // node data is here
    [SerializeField] private int MapNodeCount = 15;                         // node count is here
    private List<Vector2Int> RoomList;
    private Vector2Int RoomPoint;
    private Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up };
    private int[] oppositeIndices = { 1, 0, 3, 2 }; // 0(E) <-> 1(W), 2(S) <-> 3(N)
    public void CreateMap(Dictionary<Vector2Int, FloorData> microMap)
    {
        DefaultMap(microMap);
        while (!RoomList.Count.Equals(0) && microMap.Count < MapNodeCount)
        {
            FindDoor(microMap,RoomList[0]);
            RoomList.RemoveAt(0);
        }
    }
    public void RemoveMap(Dictionary<Vector2Int, FloorData> microMap)
    {
        microMap.Clear();
        RoomList?.Clear();
    }
    private void DefaultMap(Dictionary<Vector2Int, FloorData> microMap)
    {
        microMap.Clear();
        RoomList = new List<Vector2Int>();
        RoomPoint = Vector2Int.zero;
        microMap.Add(RoomPoint, new FloorData());           // 0번째 생성
        RoomList.Add(RoomPoint);
    }
    private void FindDoor(Dictionary<Vector2Int, FloorData> microMap,Vector2Int roomVector)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (microMap.Count >= MapNodeCount) return;
            if (UnityEngine.Random.value > 0.5f)
            {
                RoomPoint = roomVector + directions[i] * 1;
                if (!microMap.ContainsKey(RoomPoint))
                {
                    microMap.Add(RoomPoint, new FloorData());

                    microMap[roomVector].SetDoorState(i, true);
                    microMap[RoomPoint].SetDoorState(oppositeIndices[i], true);

                    microMap[roomVector].AddConnectedRoom(RoomPoint);
                    microMap[RoomPoint].AddConnectedRoom(roomVector);

                    RoomList.Add(RoomPoint);
                }
            }
        }
        if (microMap[roomVector].GetOpenDoorCount() < 1)
        {
            int forceOpenRoomNum = UnityEngine.Random.Range(0,4);
            RoomPoint = roomVector + directions[forceOpenRoomNum] * 1;
            if (!microMap.ContainsKey(RoomPoint))
            {
                microMap.Add(RoomPoint, new FloorData());

                microMap[roomVector].SetDoorState(forceOpenRoomNum, true);
                microMap[RoomPoint].SetDoorState(oppositeIndices[forceOpenRoomNum], true);

                AddRoomConnection(microMap,roomVector, RoomPoint);

                RoomList.Add(RoomPoint);
            }
            else
            {
                microMap[roomVector].SetDoorState(forceOpenRoomNum, true);
                microMap[RoomPoint].SetDoorState(oppositeIndices[forceOpenRoomNum], true);

                AddRoomConnection(microMap,roomVector, RoomPoint);
            }
        }
    }
    private void AddRoomConnection(Dictionary<Vector2Int, FloorData> microMap,Vector2Int from, Vector2Int to)
    {
        if (!microMap.ContainsKey(from) || !microMap.ContainsKey(to))
        {
            Debug.LogError($"Cannot connect {from} <-> {to}: One or both keys missing");
            return;
        }

        microMap[from].AddConnectedRoom(to);
        microMap[to].AddConnectedRoom(from);
    }
}
public class FloorData
{
    private bool[] isOpen = new bool[4];                                    // 0 : East, 1 : West, 2 : South, 3 : North
    private List<Vector2Int> connectedRoom;
    private bool isEnd = false;
    private bool isStart = false;
    private Room room;
    public FloorData()
    {
        DoorStartSetting();
        connectedRoom = new List<Vector2Int>();
        room = new Room();
    }
    public void SetRoomData(Room room)
    {
        this.room = room;
    }
    public Room GetRoomData()
    {
        return this.room;
    }
    private void DoorStartSetting()
    {
        for (int i = 0; i < isOpen.Length; i++)
        {
            isOpen[i] = false;
        }
    }
    public bool GetDoorState(int index)
    {
        return this.isOpen[index];
    }
    public void SetDoorState(int index, bool boolChange)
    {
        this.isOpen[index] = boolChange;
    }
    public int GetOpenDoorCount()
    {
        int count = 0;
        for(int i = 0; i < isOpen.Length; i++)
        {
            if (isOpen[i]) count++;
        }
        return count;
    }
    public void AddConnectedRoom(Vector2Int connectVector)
    {
        if(!connectedRoom.Contains(connectVector))
        connectedRoom.Add(connectVector);
    }
    public void RemoveConnectedRoom(Vector2Int connectVector)
    {
        connectedRoom.Remove(connectVector);
    }
    public List<Vector2Int> GetConnectedRoom()
    {
        return connectedRoom;
    }
    public void SetStartRoom()
    {
        isStart = true;
    }
    public bool getBoolStartRoom()
    {
        return isStart;
    }
    public void SetEndRoom()
    {
        isEnd = true;
    }
    public bool getBoolEndRoom()
    {
        return isEnd;
    }
}
