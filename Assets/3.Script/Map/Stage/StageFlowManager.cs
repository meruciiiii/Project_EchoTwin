
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageFlowManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameObject[] camp;
    private Dictionary<GameObject, Transform> enterTable;
    private Dictionary<NodeType, Action<StageNode>> nodeEventTable;
    private void Awake()
    {
        nodeEventTable = new Dictionary<NodeType, Action<StageNode>>{
            { NodeType.Clear, OnClearNode },
            { NodeType.Battle, OnBattleNode },
            { NodeType.Boss, OnBossNode },
            { NodeType.Recovery, OnRecoveryNode },
            { NodeType.Resource, OnResourceNode },
            { NodeType.Altar, OnAltarNode }};
        stageManager.onNodeEntered += HandleNode;
        enterTable = new Dictionary<GameObject, Transform>();
        for (int i = 0; i < camp.Length; i++)
        {
            enterTable.Add(camp[i], FindChildWithTag(camp[i]));
        }
    }
    public Transform FindChildWithTag(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag("P_SP"))
            {
                return child;
            }
        }
        return null;
    }
    public void HandleNode(StageNode node)
    {
        if (nodeEventTable.TryGetValue(node.nodeType, out Action<StageNode> action))
        {
            action.Invoke(node);
        }
    }
    private void OnClearNode(StageNode node)
    {
        Debug.Log("Clear Node Entered and is wrong entered");
    }
    private void OnBattleNode(StageNode node)
    {
        Debug.Log("Battle Start!");
        mapManager.GenerateMap();
    }
    private void OnBossNode(StageNode node)
    {
        Debug.Log("Boss Battle!");
    }
    private void OnRecoveryNode(StageNode node)
    {
        Debug.Log("Recovered");
        mapManager.StageMoving(enterTable[camp[1]].position);
    }
    private void OnResourceNode(StageNode node)
    {
        Debug.Log("Resource Acquired");
        mapManager.StageMoving(enterTable[camp[3]].position);
    }
    private void OnAltarNode(StageNode node)
    {
        Debug.Log("Altar Event");
        mapManager.StageMoving(enterTable[camp[2]].position);
    }
}
