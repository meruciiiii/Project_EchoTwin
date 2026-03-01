
using System.Collections.Generic;
using UnityEngine;

public class StageNodePopulator : MonoBehaviour
{
    public void SetETCNode(List<List<StageNode>> floors)
    {
        List<StageNode> candidates = new List<StageNode>();
        for (int floor = 1; floor < floors.Count - 1; floor++)
        {
            foreach (var node in floors[floor])
            {
                candidates.Add(node);
            }
        }
        if (candidates.Count < 3)
        {
            Debug.LogWarning("Not enough nodes to assign special types.");
            return;
        }
        Shuffle(candidates);
        candidates[0].nodeType = NodeType.Resource;
        candidates[1].nodeType = NodeType.Altar;
        candidates[2].nodeType = NodeType.Recovery;
        floors[5][0].nodeType = NodeType.Boss;
    }
    private void Shuffle(List<StageNode> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
