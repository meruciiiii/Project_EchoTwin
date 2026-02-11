using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapManager : MonoBehaviour
{
    [SerializeField] private Dictionary<Vector2Int, FloorData> microMap;          // map node is here
    [SerializeField] private MapCreater mapCreater;
    [SerializeField] private MapChecker mapChecker;
    [SerializeField] private GameObject mapDrawCanvas;
    [SerializeField] private MapDrawer mapDrawer;
    [SerializeField] private MapRoomPopulator mapRoomPopulator;
    [SerializeField] private MapTrace mapTrace;
    private void Awake()
    {
        microMap = new Dictionary<Vector2Int, FloorData>();
        if (!TryGetComponent(out mapCreater))
            Debug.Log("TryGetComponent MapCreater is fail");
        if (!TryGetComponent(out mapChecker))
            Debug.Log("TryGetComponent MapChecker is fail");
        mapDrawCanvas = GameObject.FindWithTag("MapDrawer");
        if (!mapDrawCanvas.TryGetComponent(out mapDrawer))
            Debug.Log("TryGetComponent MapDrawer is fail");
        if (!TryGetComponent(out mapTrace))
            Debug.Log("TryGetComponent MapTrace is fail");
        if (!TryGetComponent(out mapRoomPopulator))
            Debug.Log("TryGetComponent MapRoomPopulator is fail");
    }
    public void GenerateMap()
    {
        int safety = 100;
        do
        {
            mapCreater.CreateMap(microMap);
            safety--;
            if (safety <= 0)
            {
                Debug.LogError("Map generation failed");
                break;
            }
        }
        while (mapChecker.LongestCheck(microMap));
        int count = 100 - safety;
        Debug.Log("Map Create is Finished in " + count + "....................");
        mapDrawer.EnterDraw(GetMap());
        mapRoomPopulator.Populate(microMap, 1);
        Debug.Log("Populate is sucess");
    }
    public IReadOnlyDictionary<Vector2Int, FloorData> GetMap()
    {
        return microMap;
    }
    private void PlayerMove(Vector2Int currentPotint)
    {
        Vector2Int pastPotint = mapTrace.GetCurrentStayPoint();
        if (pastPotint!= new Vector2Int(-20, -20))
        mapDrawer.AlreadyStep(pastPotint);
        mapTrace.PlayerStep(currentPotint);
        mapDrawer.playerStanding(currentPotint);
    }// 플레이어가 움직이면 event에서 실행될 메서드
}
