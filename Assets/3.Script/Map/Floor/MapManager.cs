
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // ½Ì±ÛÅæ »ý¼º ¹× ÆÄ±« ºÒ°¡ ¼³Á¤
    //public static MapManager instance = null;
    //private void Awake()
    //{
    //    if(instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    DontDestroyOnLoad(gameObject);
    //} 
    private Dictionary<Vector2Int, MapData> microMap;          // map node is here
    private MapCreater mapCreater;
    private MapChecker mapChecker;
    private MapDrawer mapDrawer;
    private void Awake()
    {
        microMap = new Dictionary<Vector2Int, MapData>();
        if (!TryGetComponent(out mapCreater))
            Debug.Log("TryGetComponent MapCreater is fail");
        if (!TryGetComponent(out mapChecker))
            Debug.Log("TryGetComponent MapChecker is fail");
        if (!TryGetComponent(out mapDrawer))
            Debug.Log("TryGetComponent MapDrawer is fail");
    }
    public void GenerateMap()
    {
        do
        {
            mapCreater.CreateMap(microMap);
        }
        while (mapChecker.LongestCheck(microMap));
        mapDrawer.EnterDraw(GetMap());
    }
    public IReadOnlyDictionary<Vector2Int, MapData> GetMap()
    {
        return microMap;
    }
}
