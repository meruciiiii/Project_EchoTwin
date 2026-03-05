using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapManager : MonoBehaviour
{
    private Dictionary<Vector2Int, FloorData> microMap;          // map node is here
    private Dictionary<Vector2Int, GameObject> roomObject;
    private Dictionary<Vector2Int, List<GameObject>> enemyPool;
    private MapCreater mapCreater;
    private MapChecker mapChecker;
    private GameObject mapDrawCanvas;
    private MapDrawer mapDrawer;
    private MapRoomPopulator mapRoomPopulator;
    private RoomView roomView;
    private MapMoving mapMoving;
    private EnemySpawner enemySpawner;
    //private MapTrace mapTrace;
    [SerializeField] private RoomObjects roomObjects;
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
        if (!TryGetComponent(out roomView))
            Debug.Log("TryGetComponent RoomView is fail");
        if (!TryGetComponent(out mapMoving))
            Debug.Log("TryGetComponent MapMoving is fail");
        if (!TryGetComponent(out roomObjects))
            Debug.Log("TryGetComponent MapMoving is fail");
        if (!TryGetComponent(out enemySpawner))
            Debug.Log("TryGetComponent EnemySpawner is fail");
        //roomObjects = Resources.Load<RoomObjects>("RoomPrefabsScriptableObject");
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
        //Debug.Log("Map Create is Finished in " + count + "....................");
        roomObject = mapRoomPopulator.Populate(microMap, 1, 1, roomObjects);//please edit stage and floor
        //Debug.Log("Populate is sucess");
        enemyPool = enemySpawner.SpawnMonster(microMap, roomObject);
        GameManager.instance.setDic(enemyPool);
        //Debug.Log("enemySpawn is sucess");
        SetStartCoord();
        //Debug.Log("currentCoord is : " + currentCoord);
        mapMoving.MoveStartRooom();
        if (!roomObject.TryGetValue(currentCoord, out GameObject roomPrefab))
        {
            Debug.Log("TryGetValue roomPrefab is Error");
            return;
        }
        roomView.SetDoors(roomPrefab);
        if (!microMap.TryGetValue(currentCoord, out FloorData floor))
        {
            Debug.Log("TryGetValue roomPrefab is Error");
            return;
        }
        microMap[currentCoord].SetVisit();
        mapDrawer.EnterDraw(GetMap(), currentCoord);
        roomView.DoorAccordingState(floor);
        roomView.OnDoorUsed += PlayerTryMove;
    }
    public IReadOnlyDictionary<Vector2Int, FloorData> GetMap()
    {
        return microMap;
    }
    
    public void PlayerTryMove(Vector2Int direction)
    {
        Vector2Int target = currentCoord + direction;
        if (!microMap.TryGetValue(currentCoord, out FloorData oldFloor))
        {
            Debug.Log("oldFloor TryGetValue is Error");
            return;
        }
        if (!microMap.TryGetValue(target, out FloorData newFloor))
        {
            Debug.Log("currentCoord is Error");
            return;
        }
        roomView.DoorResetting(oldFloor);
        currentCoord = target;
        //해당 오브젝트 가져오기
        if(!roomObject.TryGetValue(currentCoord, out GameObject roomPrefab))
        {
            Debug.Log("TryGetValue roomPrefab is Error");
            return;
        }
        //이전 문에 대한 방향에 맞게 해당하는 문의 스폰위치 가져오기
        //스폰 위치 Vector3 Position
        roomView.SetDoors(roomPrefab);
        roomView.DoorAccordingState(newFloor);
        Vector3 playerSpawnPosition = roomView.GetDoor(direction);
        microMap[currentCoord].SetVisit();
        Vector3 targetPosition = playerSpawnPosition + 50 * direction.x * Vector3.left + 50 * direction.y * Vector3.forward;
        Debug.Log("start position : "+playerSpawnPosition);
        Debug.Log("end position : "+ targetPosition);
        GameManager.instance.whenMapChange(targetPosition, currentCoord);
        mapDrawer.EnterDraw(GetMap(), currentCoord);
        mapMoving.MovePlayer(playerSpawnPosition);
        //mapMoving.PlayerPush(direction);

    }// 플레이어가 움직이면 event에서 실행될 메서드
    private void SetStartCoord()
    {
        foreach (KeyValuePair<Vector2Int, FloorData> pair in microMap)
        {
            if (pair.Value.getBoolStartRoom())
            {
                //Debug.Log("Start Room type check : "+pair.Value.GetRoomData().GetRoomType());
                //Debug.Log("Start Room object name : "+ roomObject[pair.Key].name);
                currentCoord = pair.Key;
                break;
            }
        }
    }
    public void StageMoving(Vector3 enterPosition)
    {
        //Debug.Log(enterPosition + " : enter this position");
        mapMoving.MovePlayer(enterPosition);
    }
    public void MapClear()
    {
        foreach(KeyValuePair<Vector2Int, List<GameObject>> enemyList in enemyPool)
        {
            if (enemyList.Value != null)
            {
                foreach(GameObject enemy in enemyList.Value)
                {
                    Destroy(enemy);
                }
            }
        }
        microMap.Clear();
        roomObject.Clear();
        enemyPool.Clear();
    }
    private void PlayerInBattle()
    {
        if (!microMap[currentCoord].GetRoomData().GetRoomType().Equals(RoomType.Battle))
            return;
    }
}
