using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChecker : MonoBehaviour
{


    private List<Vector2Int> connectedRoom;
    private List<Vector2Int> singleDoorRoom = new List<Vector2Int>();
    private Queue<Vector2Int> adjacentNode = new Queue<Vector2Int>();
    private Queue<Vector2Int> tempNode = new Queue<Vector2Int>();
    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
    private Queue<int> depthNode = new Queue<int>();
    private Vector2Int startNode = Vector2Int.zero;
    private Vector2Int furthestNode = Vector2Int.zero;
    private int deepestDistance = 0;
    private bool bossRoomisTop = false;
    private bool checker;
    public bool LongestCheck(Dictionary<Vector2Int, MapData> microMap)
    {
        checker = false;
        if(microMap.Count < 15)                                                             // 방 개수 정하기
        {
            //Debug.Log("개수 부족해서 실행합니다아아아아아");
            return true;
        }
        //MapCreater와 Dictionary - microMap를 불러오기 위한 작업
        //Dictionary - microMap 중 문 한개인 방 들에 대한 리스트 생성
        //Debug.Log(microMap.Count);
        SingleDoorRoom(microMap);
        foreach (Vector2Int singleDoorRoom in singleDoorRoom)
        {
            DepthSerch(microMap, singleDoorRoom);
        }
        deepestDistance = 0;
        if (microMap.ContainsKey(startNode))
            microMap[startNode].SetStartRoom();
        startNode = Vector2Int.zero;
        if (microMap.ContainsKey(furthestNode))
            microMap[furthestNode].SetEndRoom();
        furthestNode = Vector2Int.zero;
        return checker;
    }
    public void DepthSerch(Dictionary<Vector2Int, MapData> microMap, Vector2Int startNode)
    {
        //노드를 저장할 공간, 노드의 깊이를 저장할 공간, 가장 먼 노드, 가장 먼 노드와의 거리
        adjacentNode.Enqueue(startNode);
        depthNode.Enqueue(0);
        visited.Add(startNode);
        Vector2Int current = Vector2Int.zero;
        int currentDepth = 0;
        while (adjacentNode.Count > 0)
        {
            current = adjacentNode.Dequeue();
            currentDepth = depthNode.Dequeue();
            if (!microMap.TryGetValue(current, out MapData mapData))
                continue;
            connectedRoom = mapData.GetConnectedRoom();

            foreach (Vector2Int next in connectedRoom)
            {
                if (visited.Add(next))
                {
                    adjacentNode.Enqueue(next);
                    depthNode.Enqueue(currentDepth + 1);
                }
            }
        }
        visited.Clear();
        if (deepestDistance < currentDepth)
        {
            this.startNode = startNode;
            deepestDistance = currentDepth;
            furthestNode = current;
        }
    }
    public void SingleDoorRoom(Dictionary<Vector2Int, MapData> microMap)
    {
        singleDoorRoom.Clear();
        foreach (KeyValuePair<Vector2Int, MapData> drawMap in microMap)
        {
            if (drawMap.Value.GetOpenDoorCount() < 2)
            {
                DoorisDown(drawMap);
                if(bossRoomisTop)
                    singleDoorRoom.Add(drawMap.Key);
                //Debug.Log("Door is one in the room : "+drawMap.Key);
            }
        }
        //Debug.Log(singleDoorRoom.Count);
        if(singleDoorRoom.Count < 1)
        {
            checker = true;
            return;
        }
        startNode = singleDoorRoom[0];
    }
    public void DoorisDown(KeyValuePair<Vector2Int, MapData> drawMap)
    {
        bossRoomisTop = drawMap.Value.GetDoorState(2);
    }
}
