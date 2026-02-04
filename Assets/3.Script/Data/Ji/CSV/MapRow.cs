using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRow
{
    public int ID;
    public string name;
    public string floorTilePath;
    public string wallTilePaht;
    public List<int> poolIDs;
    public int bossID;
    public int minRoom;
    public int maxRoom;
}
