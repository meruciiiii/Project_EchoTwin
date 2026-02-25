using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DropItemDatabase")]
public class ItemDataBase : ScriptableObject
{
    public GameObject goldPrefab;
    public GameObject cristalPrefab;
    public GameObject heartPrefab;

    private static ItemDataBase instance;
    public static ItemDataBase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ItemDataBase>("DropItemDatabase");
                if(instance == null)
                {
                    Debug.Log("instance null");
                }
            }
            return instance;
        }
    }
}
