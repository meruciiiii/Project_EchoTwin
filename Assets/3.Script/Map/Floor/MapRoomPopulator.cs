using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapRoomPopulator : MonoBehaviour
{
    int eventRoomCount;
    int eliteRoomCount;
    int eliteRoomProbability;
    int roomID;
    int floor;
    Room.RoomType roomType;

    public void Populate(Dictionary<Vector2Int, FloorData> microMap, int floor)
    {
        this.floor = floor;
        RoomCondition();
        //room 생성
        foreach(KeyValuePair<Vector2Int, FloorData> room in microMap)
        {
            CreateRoom(room.Value);
        }
        //연결

    }
    private void CreateRoom(FloorData floor)
    {
        DecisionType(floor);
        floor.GetRoomData().SetRoom(DecisionRoomID(),this.floor, DecisionMonsterPackID(), roomType);
    }
    private int DecisionRoomID()
    {
        int choice = 0;
        if (roomType.Equals(Room.RoomType.Start))
        {
            choice = 101;
        }
        else if (roomType.Equals(Room.RoomType.Battle))
        {
            choice = UnityEngine.Random.Range(1, 18);
        }
        else if (roomType.Equals(Room.RoomType.Shop))
        {
            choice = 102;
        }
        else if (roomType.Equals(Room.RoomType.Forge))
        {
            choice = 103;
        }
        else if (roomType.Equals(Room.RoomType.Elite))
        {
            choice = UnityEngine.Random.Range(19, 20);
        }
        else if (roomType.Equals(Room.RoomType.Reward))
        {
            choice = 104;
        }
        else if (roomType.Equals(Room.RoomType.Boss))
        {
            if (floor.Equals(1))
                choice = 21;
            else if (floor.Equals(2))
                choice = 22;
            else
                choice = 0;
        }
        return choice;
    }
    private int DecisionMonsterPackID()
    {
        int choice = 0;
        if (roomType.Equals(Room.RoomType.Battle))
        {
            choice = UnityEngine.Random.Range(1, 9);
        }
        return choice;
    }
    private void DecisionType(FloorData floor)
    {
        int choice = UnityEngine.Random.Range(1, ((int)Room.RoomType.count)-3);
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
            if (eliteRoomCount > 0)
                choice = 1;
            else
            {
                if (this.floor > 4)
                {
                    if(this.floor > 8)
                    {
                        choice = 1;
                    }
                    else
                    {
                        if (UnityEngine.Random.value*100 < eliteRoomProbability)
                            eliteRoomCount++;
                        else
                            choice = 1;
                    }
                }
            }
        }
        if (floor.getBoolStartRoom())
            choice = 0;
        if (floor.getBoolEndRoom())
            choice = 5;
        roomType = (Room.RoomType)choice;
    }//Start, Battle, Shop, Forge, Elite, Reward, Boss, count
    private void RoomCondition()
    {
        eventRoomCount = 0;
        eliteRoomCount = 0;
        eliteRoomProbability = 30;
    }
}
