
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefabList : MonoBehaviour
{
    private List<GameObject> RoomPrefabs;
    private void Awake()
    {
        RoomPrefabs = new List<GameObject>();
        string roomTag = "R_";
        for(int i = 1; i<21; i++)
        {
            if(i < 10)
            roomTag = "R_0" + i;
            else
                roomTag = "R_" + i;
            RoomPrefabs.Add(GameObject.FindGameObjectWithTag(roomTag));
        }
    }
}
 