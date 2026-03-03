
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private StageMap stageMap;
    private StageCreater stageCreater;
    private StageNodePopulator stageNodePopulator;
    [SerializeField] private StageNodeView stageNodeView;
    private NodeData currentNode;
    private StagePin stagePin;
    private void Awake()
    {
        if(!TryGetComponent(out stageCreater))
        {
            Debug.Log("TryGetComponent StageCreater is fail");
        }
        if(!TryGetComponent(out stageNodePopulator))
        {
            Debug.Log("TryGetComponent StageNodePopulator is fail");
        }
        stageNodeView.OnNodeClicked += SelectNode;
        if (!TryGetComponent(out stagePin))
        {
            Debug.Log("TryGetComponent StagePin is fail");
        }
    }

    public void SelectNode(NodeData node)
    {
        //if (!CanMove(node)) return;

        currentNode = node;
        MovePlayerIcon(node);
    }
    private void MovePlayerIcon(NodeData node)
    {
        // playerIcon 이동 처리
    }
    public void GenerateStage()
    {
        stageMap = stageCreater.StageCreat();
        stageNodePopulator.SetETCNode(stageMap.floors);
        stageNodeView.DrawConnections(stageMap.floors);
    }
}
