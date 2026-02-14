using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapManager : MonoBehaviour
{
    private Dictionary<Vector2Int, FloorData> microMap;          // map node is here
    private MapCreater mapCreater;
    private MapChecker mapChecker;
    private GameObject mapDrawCanvas;
    private MapDrawer mapDrawer;
    private MapRoomPopulator mapRoomPopulator;
    private MapMoving mapMoving;
    //private MapTrace mapTrace;
    [SerializeField] private RoomPrefabs roomPrefabs;
    [SerializeField] private Vector2Int currentCoord;

    //오브젝트 연결 필요(맵 순서에 맞춰서, 노드들에 입히는것 중복 없게 만들기)
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
        if (!TryGetComponent(out mapRoomPopulator))
            Debug.Log("TryGetComponent MapRoomPopulator is fail");
        if (!TryGetComponent(out mapMoving))
            Debug.Log("TryGetComponent MapMoving is fail");
        roomPrefabs = Resources.Load<RoomPrefabs>("RoomPrefabsScriptableObject");
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
        SetStartCoord();
        if (!microMap.TryGetValue(currentCoord, out FloorData floor))
        {
            Debug.Log("currentCoord is Error");
            return;
        }
        mapMoving.ExecuteMove(Vector2Int.zero, floor.GetRoomData());
        
    }
    public IReadOnlyDictionary<Vector2Int, FloorData> GetMap()
    {
        return microMap;
    }
    
    public void PlayerTryMove(Vector2Int direction)
    {
        Vector2Int target = currentCoord + direction;
        if (!microMap.TryGetValue(target, out FloorData floor))
        {
            return;
        }

        currentCoord = target;
        mapMoving.ExecuteMove(direction, floor.GetRoomData());

    }// 플레이어가 움직이면 event에서 실행될 메서드
    private void SetStartCoord()
    {
        foreach (KeyValuePair<Vector2Int, FloorData> pair in microMap)
        {
            if (pair.Value.getBoolStartRoom())
            {
                currentCoord = pair.Key;
                break;
            }
        }
    }
    private void ConnectDoors()
    {
        DoorTrigger[] doors = FindObjectsOfType<DoorTrigger>();

        foreach (var door in doors)
        {
            door.onPlayerEnter += PlayerTryMove;
        }
    }
}
