using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Room
{
    private int roomID;
    public enum RoomType
    {
        Start, Battle, Shop, Forge, Elite, Reward, Boss, count
    }
    private float floor;
    private int monsterPackID;
    private Vector2[] spawnPoints;
    private RoomType roomType;
    public Room()
    {
        roomID = 0;
        floor = 1;
        monsterPackID = 1;
        roomType = RoomType.Start;
    }
    public void SetRoom(int roomID, int floor, int monsterPackID, RoomType roomType)
    {
        this.roomID = roomID;
        this.floor = floor;
        this.monsterPackID = monsterPackID;
        this.roomType = roomType;
    }
    public void SetStartRoom()
    {
        this.roomType = RoomType.Start;
    }
    public Vector2[] GetSpawnPoints()
    {
        spawnPoints = new Vector2[4];
        return spawnPoints;
    }
}
