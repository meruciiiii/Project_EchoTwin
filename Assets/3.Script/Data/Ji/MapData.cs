using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Map Data")]
public class MapData : ScriptableObject
{
    [Header("ID")]
    public int id;
    public string mapName;

    [Header("Sprite")]
    public Sprite floorTile;
    public Sprite wallTile;

    [Header("Enemy")]
    public List<int> poolIDs;
    public int bossID;

    [Header("Room Amount")]
    public int minRoom;
    public int maxRoom;
}
