
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreater : MonoBehaviour
{
    private StageChecker stageChecker;
    private int totalFloor = 6;
    private int[] counts;
    public StageMap StageCreat()
    {
        if (!TryGetComponent(out stageChecker))
        {
            Debug.Log("TryGetComponent StageChecker is fail");
        }
        if (totalFloor != 6)
        {
            Debug.LogError("현재 구조는 6층 전용입니다.");
            return null;
        }
        FloorCount();
        StageMap stageMap;
        int safety = 0;
        do
        {
            stageMap = new StageMap();
            StageNodeCreate(stageMap);
            List<int[]> paths = CreatePaths();
            foreach (int[] path in paths)
            {
                ConnectNodes(path, stageMap.floors);
            }
            safety++;
            if (safety > 50)
            {
                Debug.LogWarning("맵 생성 재시도 초과");
                break;
            }

        } while (!stageChecker.HasTooManyBranches(stageMap.floors));
        return stageMap;
    }
    private void FloorCount()
    {
        counts = new int[totalFloor];
        counts[0] = 1;
        counts[1] = 2;
        counts[2] = 3;
        counts[3] = 4;
        counts[4] = 4;
        counts[5] = 1;
    }
    private void StageNodeCreate(StageMap stageMap)
    {
        stageMap.floors = new List<List<StageNode>>();
        for (int i = 0; i < totalFloor; i++)
        {
            List<StageNode> stageNodes = new List<StageNode>();
            for (int j = 0; j < counts[i]; j++)
            {
                StageNode stageNode = new StageNode();
                stageNode.floorIndex = i;
                stageNode.nodeIndex = j;
                stageNodes.Add(stageNode);
            }
            stageMap.floors.Add(stageNodes);
        }
    }
    private void ConnectNodes(int[] path, List<List<StageNode>> floors)
    {
        //stageMap.floors = new List<List<StageNode>>();
        /*  StageNode
            public int floorIndex;
            public int nodeIndex;
            public List<StageNode> nextNodes;
         */

        for (int floor = 0; floor < path.Length - 1; floor++)
        {
            StageNode currentNode = floors[floor][path[floor]];
            StageNode nextNode = floors[floor + 1][path[floor + 1]];
            currentNode.nextNodes.Add(nextNode);
        }
    }
    private List<int[]> CreatePaths()
    {
        int totalFloor = counts.Length;
        List<int[]> paths = new List<int[]>();
        HashSet<string> pathSet = new HashSet<string>();
        bool[][] visited = new bool[totalFloor][];
        for (int i = 0; i < totalFloor; i++)
            visited[i] = new bool[counts[i]];
        int safety = 0;
        int safetyLimit = 100;
        while (!AllNodesCovered(visited) && safety < safetyLimit)
        {
            safety++;
            int[] path = new int[totalFloor];
            path[0] = 0;
            int prevIndex = path[0];
            for (int floor = 1; floor < totalFloor - 1; floor++)
            {
                List<int> candidates = new List<int>();
                for (int offset = 0; offset <= 1; offset++)
                {
                    int idx = prevIndex + offset;
                    if (idx >= 0 && idx < counts[floor])
                        candidates.Add(idx);
                }
                List<int> unvisited = new List<int>();
                foreach (int idx in candidates)
                {
                    if (!visited[floor][idx])
                        unvisited.Add(idx);
                }
                int nextIndex;
                if (unvisited.Count > 0)
                {
                    nextIndex = unvisited[UnityEngine.Random.Range(0, unvisited.Count)];
                }
                else
                {
                    nextIndex = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                }
                path[floor] = nextIndex;
                prevIndex = nextIndex;
            }
            path[totalFloor - 1] = 0;
            string key = string.Join(",", path);
            if (pathSet.Add(key))
            {
                paths.Add(path);
                for (int floor = 0; floor < totalFloor; floor++)
                {
                    visited[floor][path[floor]] = true;
                }
            }
        }

        if (safety >= safetyLimit)
            Debug.LogWarning("모든 노드를 방문하기 전에 안전 제한에 도달했습니다.");

        return paths;
    }
    private bool AllNodesCovered(bool[][] visited)
    {
        for (int floor = 0; floor < visited.Length; floor++)
        {
            for (int i = 0; i < visited[floor].Length; i++)
            {
                if (!visited[floor][i])
                    return false;
            }
        }
        return true;
    }
}
