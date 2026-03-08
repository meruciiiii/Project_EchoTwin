
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageFlowManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameObject[] camp;
    [SerializeField] private SceneTransition sceneTransition;

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
        stageManager.onPortalEntered += OnPortalEntered;
        enterTable = new Dictionary<GameObject, Transform>();
        for (int i = 0; i < camp.Length; i++)
        {
            enterTable.Add(camp[i], FindChildWithTag(camp[i]));
        }
    }
    public void StartStage()
    {
        mapManager.GenerateMap(1);
        stageManager.GenerateStage();
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
        sceneTransition.PlayFullTransition(() => {
            mapManager.GenerateMap(node.floorIndex); 
            stageManager.SetNodeUI(false);           
        });
    }
    private void OnBossNode(StageNode node)
    {
        Debug.Log("Boss Battle!");
        Debug.Log("node.floorIndex = "+ node.floorIndex + "!");
        sceneTransition.PlayFullTransition(() => {
            mapManager.GenerateMap(node.floorIndex);
            stageManager.SetNodeUI(false);
        });
    }
    private void OnRecoveryNode(StageNode node)
    {
        Debug.Log("Recovered");
        sceneTransition.PlayFullTransition(() => {
        mapManager.StageMoving(enterTable[camp[1]].position); 
        stageManager.SetNodeUI(false); 
        });
    }
    private void OnResourceNode(StageNode node)
    {
        Debug.Log("Resource Acquired");
        sceneTransition.PlayFullTransition(() => {
        mapManager.StageMoving(enterTable[camp[3]].position); 
        stageManager.SetNodeUI(false); 
        });
    }
    private void OnAltarNode(StageNode node)
    {
        Debug.Log("Altar Event");
        sceneTransition.PlayFullTransition(() => {
        mapManager.StageMoving(enterTable[camp[2]].position); 
        stageManager.SetNodeUI(false); 
        });
    }
    private void OnPortalEntered()
    {
        sceneTransition.PlayFullTransition(() => {
        mapManager.MapClear();       
        stageManager.SetNodeUI(true); 
        });
    }
}
