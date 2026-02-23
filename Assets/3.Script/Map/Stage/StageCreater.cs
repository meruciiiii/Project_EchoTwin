
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreater : MonoBehaviour
{
    private StageMap stageMap = new StageMap();
    [SerializeField] private int totalFloor = 6;
    
    private int[] counts;
    public void StageCreat()
    {
        if (totalFloor != 6)
        {
            Debug.LogError("현재 구조는 6층 전용입니다.");
            return;
        }
        FloorCount();
        StageNodeCreate();
        ConnectNodes();
    }
    private void FloorCount()
    {
        counts = new int[totalFloor];
        counts[0] = 1;
        counts[1] = 2;
        counts[2] = UnityEngine.Random.Range(3, 5);  // 3 or 4
        counts[3] = 4;
        counts[4] = UnityEngine.Random.Range(2, 4);  // 2 or 3
        counts[5] = 1;
    }
    private void StageNodeCreate()
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
    private void ConnectNodes()
    {
        //stageMap.floors = new List<List<StageNode>>();
        /*  StageNode
            public int floorIndex;
            public int nodeIndex;
            public List<StageNode> nextNodes;
         */
        stageMap.floors[0][0].nextNodes = stageMap.floors[1];
        foreach(StageNode nextNodes in stageMap.floors[stageMap.floors.Count - 1])
        {
            nextNodes.nextNodes = stageMap.floors[stageMap.floors.Count];
        }
    }
    private void CreatePath()
    {
        List<int[]> paths;
        int pathCount = UnityEngine.Random.Range(3, 5); // 3~4
        for(int i = 0; i < pathCount; i++)
        {
            int nextIndex = UnityEngine.Random.Range(-1, 2);
            if (i.Equals(0))
            {
                nextIndex = 0;
            }
        }
    }
}
