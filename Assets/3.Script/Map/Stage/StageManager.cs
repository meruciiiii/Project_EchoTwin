
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
    [SerializeField] private PortalManager portalManager;

    public event Action<StageNode> onNodeEntered;
    public event Action onPortalEntered;
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
        stageNodeView.onNodeEntered += OnNodeEntered;
        portalManager.onPortalEntered += OnPortalEntered;
    }
    public void GenerateStage()
    {
        stageMap = stageCreater.StageCreat();
        stageNodePopulator.SetETCNode(stageMap.floors);
        stageNodeView.DrawConnections(stageMap.floors);
    }
    private void OnNodeEntered(StageNode node)
    {
        onNodeEntered?.Invoke(node);
    }
    private void OnPortalEntered()
    {
        onPortalEntered?.Invoke();
        stageNodeView.gameObject.SetActive(true);
    }
} 
