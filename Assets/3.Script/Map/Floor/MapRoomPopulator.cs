
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoomPopulator : MonoBehaviour
{
    int eventRoomCount;
    int eliteRoomCount;
    int eliteRoomProbability;
    int floor;

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
        floor.GetRoomData().SetRoom(DecisionRoomID(),this.floor, DecisionMonsterPackID(), DecisionType(floor));
    }
    private int DecisionRoomID()
    {
        int choice = UnityEngine.Random.Range(0, ((int)Room.RoomType.count));
        return choice;
    }
    private int DecisionMonsterPackID()
    {
        int choice = UnityEngine.Random.Range(0, ((int)Room.RoomType.count));
        return choice;
    }
    private Room.RoomType DecisionType(FloorData floor)
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
        return (Room.RoomType)choice;
    }//Start, Battle, Shop, Forge, Elite, Reward, Boss, count
    private void RoomCondition()
    {
        eventRoomCount = 0;
        eliteRoomCount = 0;
        eliteRoomProbability = 30;
    }
}
