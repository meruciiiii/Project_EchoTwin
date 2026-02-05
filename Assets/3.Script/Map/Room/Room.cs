
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private int roomID;
    private enum RoomType
    {
        Start, Battle, Shop, Forge, Elite, Reward
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
    public Vector2[] GetSpawnPoints()
    {
        spawnPoints = new Vector2[4];
        return spawnPoints;
    }
}
