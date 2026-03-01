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
            CreatePaths(stageMap.floors);
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
    private void CreatePaths(List<List<StageNode>> floors)
    {
        for (int floor = 0; floor < floors.Count - 2; floor++)
        {
            List<StageNode> current = floors[floor];
            List<StageNode> next = floors[floor + 1];
            int currentCount = current.Count;
            int nextCount = next.Count;
            int[] hasPrev = new int[nextCount];
            int hasNext = 0;
            Connect(floors, floor, 0, floor + 1, 0);
            hasPrev[0]++;
            hasNext++;
            Connect(floors, floor, currentCount - 1, floor + 1, nextCount - 1);
            hasPrev[nextCount - 1]++;
            hasNext++;
            int branchFrom = UnityEngine.Random.Range(0, currentCount);
            if (branchFrom.Equals(0))
            {
                Connect(floors, floor, 0, floor + 1, 1);
                hasPrev[1]++;
            }
            else if (branchFrom.Equals(currentCount - 1))
            {
                Connect(floors, floor, currentCount - 1, floor + 1, nextCount - 2);
                hasPrev[nextCount - 2]++;
            }
            else
            {
                int branchTo = UnityEngine.Random.Range(branchFrom - 1, branchFrom + 1);
                if (currentCount - 2 < nextCount - 2)// currentCount - 2 : nextCount -2
                {
                    branchTo = branchFrom;
                }
                Connect(floors, floor, branchFrom, floor + 1, branchTo);
                Connect(floors, floor, branchFrom, floor + 1, branchTo + 1);
                hasPrev[branchTo]++;
                hasPrev[branchTo + 1]++;
                hasNext++;
            }
            for (int i = 1; i < currentCount - 1; i++)
            {
                if (i.Equals(branchFrom))
                {
                    continue;
                }
                int hasPrevCount = 0;
                int nullContectPoint = 0;
                for(int j = 0; j < hasPrev.Length; j++)
                {
                    if (hasPrev[j] > 0)
                    {
                        hasPrevCount++;
                    }
                    else
                    {
                        if (nullContectPoint.Equals(0))
                            nullContectPoint = j;
                    }
                }
                if ((currentCount - hasNext).Equals(nextCount-hasPrevCount))
                {
                    Connect(floors, floor, i, floor + 1, nullContectPoint);
                    hasPrev[nullContectPoint]++;
                    hasNext++;
                    continue;
                }
                List<int> canNext = new List<int>();
                canNext.Add(i);
                if (!floors[floor][i - 1].nextNodes.Contains(floors[floor + 1][i])&& floors[floor][i - 1].nextNodes.Count>0)
                {
                    canNext.Add(i-1);
                }
                if (!floors[floor][i + 1].nextNodes.Contains(floors[floor + 1][i]) && floors[floor][i + 1].nextNodes.Count > 0)
                {
                    canNext.Add(i+1);
                }
                int findPoint = UnityEngine.Random.Range(0, canNext.Count);
                Connect(floors, floor, i, floor + 1, canNext[findPoint]);
                hasPrev[canNext[findPoint]]++;
                hasNext++;
            }
        }
        int lastConectFloor = totalFloor - 2;
        for (int i = 0; i < counts[lastConectFloor]; i++)
        {
            Connect(floors, lastConectFloor, i, lastConectFloor + 1, 0);
        }
    }
    private void Connect(List<List<StageNode>> floors, int fromFloor, int fromIndex, int toFloor, int toIndex)
    {
        StageNode fromNode = floors[fromFloor][fromIndex];
        StageNode toNode = floors[toFloor][toIndex];

        if (!fromNode.nextNodes.Contains(toNode))
        {
            fromNode.nextNodes.Add(toNode);
        }
    }
}
  