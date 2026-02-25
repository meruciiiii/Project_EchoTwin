
using System.Collections.Generic;
using UnityEngine;

public class StageChecker : MonoBehaviour
{
    public bool HasTooManyBranches(List<List<StageNode>> floors)
    {
        bool hasAnyBranch = false;
        for (int floor = 0; floor < floors.Count - 1; floor++)
        {
            int branchCount = 0;
            foreach (StageNode node in floors[floor])
            {
                if (node.nextNodes.Count > 1)
                {
                    branchCount++;
                    hasAnyBranch = true;
                }
            }
            if (branchCount > 1)
            {
                return false;
            }
        }
        return hasAnyBranch;
    }
}
