using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapRoomPopulator : MonoBehaviour
{
    private int eventRoomCount;
    private int eliteRoomCount;
    private int eliteRoomProbability;
    private int roomID;
    private int[] batteRoomID;
    private int nextRoomIDNum;
    private int stage;
    private int floor;
    private Vector2Int keyForBoss;
    private RoomType roomType;
    private RoomObjects roomObjects;
    [SerializeField] private FloorScriptableObject floorScriptable;
    private System.Random rnd = new System.Random();

    public Dictionary<Vector2Int, GameObject> Populate(Dictionary<Vector2Int, FloorData> microMap, int stage, int floor, RoomObjects roomObjects)
    {
        this.stage = stage;
        this.floor = floor;
        RoomCondition();
        //room 생성
        this.roomObjects = roomObjects;
        SetBattleRoomNum();
        nextRoomIDNum = 0;
        Dictionary<Vector2Int, GameObject> roomObject = new Dictionary<Vector2Int, GameObject>(); ;
        foreach (KeyValuePair<Vector2Int, FloorData> room in microMap)
        {
            CreateRoom(room);
            nextRoomIDNum++;
            roomObject.Add(room.Key, MappingRoom(room.Value.GetRoomData()));
        }
        if(floor > 4)
        {
            List<Vector2Int> bossConnected = microMap[keyForBoss].GetConnectedRoom();
            Vector2Int keyForReward = Vector2Int.zero;
            if (!bossConnected.Count.Equals(0))
            {
                keyForReward = bossConnected[0];
                roomObject.Remove(keyForReward);
                SetBossRoom(microMap[keyForReward]);
                roomObject.Add(keyForReward, MappingRoom(microMap[keyForReward].GetRoomData()));
                //Debug.Log("Boss and Reward success");
            }
        }
        //연결
        //Debug.Log("eliteRoomCount : " + eliteRoomCount);
        return roomObject;
    }
    private void SetBossRoom(FloorData floor)
    {
        floor.GetRoomData().SetRoom(104, this.floor, floorScriptable.rooms[104 - 79].monsterPackID,RoomType.Reward);
    }
    private void CreateRoom(KeyValuePair<Vector2Int, FloorData> room)
    {
        DecisionType(room);
        room.Value.GetRoomData().SetRoom(roomID = DecisionRoomID(), this.floor, DecisionMonsterPackID(), roomType) ;
    }
    private int DecisionRoomID()
    {
        int choice = 0;
        if (roomType.Equals(RoomType.Start))
        {
            choice = 101;
        }
        else if (roomType.Equals(RoomType.Battle))
        {
            choice = batteRoomID[nextRoomIDNum];
        }
        else if (roomType.Equals(RoomType.Shop))
        {
            choice = 102;
        }
        else if (roomType.Equals(RoomType.Forge))
        {
            choice = 103;
        }
        else if (roomType.Equals(RoomType.Elite))
        {
            choice = UnityEngine.Random.Range(19, 21);
        }
        else if (roomType.Equals(RoomType.Reward))
        {
            choice = 104;
        }
        else if (roomType.Equals(RoomType.Boss))
        {
            if (stage.Equals(1))
                choice = 21;
            else if (stage.Equals(2))
                choice = 22;
            else
                choice = 0;
        }
        return choice;
    }
    private void SetBattleRoomNum()
    {
        batteRoomID = new int[18];
        int length = batteRoomID.Length;
        for (int i = 0; i < length; i++)
        {
            batteRoomID[i] = i + 1;
        }
        for (int i = 0; i < length; i++)
        {
            int k = rnd.Next(i + 1);
            int temp = batteRoomID[k];
            batteRoomID[k] = batteRoomID[i];
            batteRoomID[i] = temp;
        }
    }
    private int DecisionMonsterPackID()
    {
        if (roomID < 23)
            return floorScriptable.rooms[roomID - 1].monsterPackID;
        else
            return floorScriptable.rooms[roomID - 79].monsterPackID;
    }
    private void DecisionType(KeyValuePair<Vector2Int, FloorData> room)
    {
        FloorData floor = room.Value;
        int choice = UnityEngine.Random.Range(1, ((int)RoomType.count) - 2);
        if (choice.Equals(2) || choice.Equals(3))
        {
            if (eventRoomCount > 0)
            {
                choice = 1;
            }
            else
                eventRoomCount++;
        }
        if (choice.Equals(4))
        {
            if (this.floor <= 1)
            {
                choice = 1;
            }
            else if (eliteRoomCount > 0)
            {
                choice = 1;
            }
            else
            {
                if (this.floor > 3)
                {
                    eliteRoomCount++;
                }
                else
                {
                    if (UnityEngine.Random.value * 100 < eliteRoomProbability)
                        eliteRoomCount++;
                    else
                        choice = 1;
                }
            }
        }
        if (floor.getBoolStartRoom())
            choice = 0;

        if (floor.getBoolEndRoom())
            choice = 5;
        if (this.floor > 4)
        {
            if (floor.getBoolEndRoom())
            {
                choice = 6;
                keyForBoss = room.Key;
            }
        }
        roomType = (RoomType)choice;
    }//Start, Battle, Shop, Forge, Elite, Reward, Boss, count
    private void RoomCondition()
    {
        eventRoomCount = 0;
        eliteRoomCount = 0;
        eliteRoomProbability = 30;
    }
    private GameObject MappingRoom(Room room)
    {
        if (room.GetRoomType().Equals(RoomType.Start))
        {
            return roomObjects.physicalStartRoom;
        }
        else if (room.GetRoomType().Equals(RoomType.Battle))
        {
            return roomObjects.physicalBattleRoom[room.GetRoomID()-1];
        }
        else if (room.GetRoomType().Equals(RoomType.Shop))
        {
            return roomObjects.physicalShopRoom;
        }
        else if (room.GetRoomType().Equals(RoomType.Forge))
        {
            return roomObjects.physicalForgeRoom;
        }
        else if (room.GetRoomType().Equals(RoomType.Elite))
        {
            if (room.GetRoomID().Equals(19))
                return roomObjects.physicalEliteRoom[0];
            else if (room.GetRoomID().Equals(20))
                return roomObjects.physicalEliteRoom[1];
        }
        else if (room.GetRoomType().Equals(RoomType.Reward))
        {
            return roomObjects.physicalRewardRoom;
        }
        else if (room.GetRoomType().Equals(RoomType.Boss))
        {
            if (stage.Equals(1))
            {
                return roomObjects.physicalFirstBossRoom;
            }
            else if (stage.Equals(2))
            {
                return roomObjects.physicalSecondBossRoom;
            }
            else
            {
                Debug.Log("Int stage is error");
                return roomObjects.physicalFirstBossRoom;
            }
        }
        Debug.Log("Room.RoomType roomType is error");
        return roomObjects.physicalStartRoom;
    }
}
