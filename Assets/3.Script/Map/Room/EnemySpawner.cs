
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonsterPackList monsterPackList;
    public Dictionary<Vector2Int, List<GameObject>> SpawnMonster(Dictionary<Vector2Int, FloorData> microMap, Dictionary<Vector2Int, GameObject> roomObject)
    {
        Dictionary<Vector2Int, List<GameObject>> enemyPool = new Dictionary<Vector2Int, List<GameObject>>();
        foreach (KeyValuePair<Vector2Int, FloorData> pair in microMap)
        {
            Vector2Int pos = pair.Key;
            FloorData data = pair.Value;
            Room room = data.GetRoomData();
            // 몬스터 팩 넘버 가져오기
            int monsterPackID = room.GetMonsterPackID();
            // 몬스터 팩 넘버에 해당하는 몬스터 리스트 가져오기
            if (monsterPackID <= 0 || monsterPackID > monsterPackList.monsterPacks.Length)
            {
                Debug.Log($"잘못된 MonsterPackID: {monsterPackID}");
                continue;
            }
            List<MonsterData> monsterDatas = monsterPackList.monsterPacks[monsterPackID - 1].monsterDataList;
            // 스폰하기(인스턴시에이트)
            //  - 스폰 좌표 찾기
            List<GameObject> spawnPoint = new List<GameObject>();
            foreach (Transform child in roomObject[pos].transform)
            {
                if (child.CompareTag("M_SP"))
                {
                    foreach(Transform childchild in child)
                    {
                        spawnPoint.Add(childchild.gameObject);
                    }
                }
            }
            
            //  - Instantiate로 생성하기
            List<GameObject> monsterPool = new List<GameObject>();
            foreach(MonsterData monsterData in monsterDatas)
            {
                for(int i = 0; i < monsterData.count; i++)
                {
                    // 생성할 위치와 회전값 설정
                    if (spawnPoint.Count.Equals(0))
                        break;
                    int rnd = UnityEngine.Random.Range(0, spawnPoint.Count);    // 랜덤한 스폰 포인트 불러오기 위한

                    Vector3 spawnPosition = spawnPoint[rnd].transform.position;
                    Quaternion spawnRotation = spawnPoint[rnd].transform.rotation;

                    // 프리팹 생성!
                    monsterPool.Add(Instantiate(monsterData.monsterPrefab, spawnPosition, spawnRotation));
                    spawnPoint.RemoveAt(rnd);
                }
            }
            enemyPool.Add(pos, monsterPool);
        }
        return enemyPool;
    }
}