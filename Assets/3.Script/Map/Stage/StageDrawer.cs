
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDrawer : MonoBehaviour
{
    public void Matching(List<List<StageNode>> floors, NodeContainer nodeContainer)
    {
        for (int i = 0; i < floors.Count; i++)
        {
            List<StageNode> stageNodes = floors[i];
            for (int j = 0; j < stageNodes.Count; j++)
            {
                StageNode stageNode = stageNodes[j];
                NodeData nodeData = nodeContainer.floors[i].Floor[j];
                Transform parentTransform = nodeData.positionTarget.transform;
                Vector3 spawnPosition = parentTransform.position;
                Quaternion spawnRotation = parentTransform.rotation;
                NodeType nodeType = stageNode.nodeType;
                if (nodeType.Equals(NodeType.Altar))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[0], spawnPosition, spawnRotation, parentTransform);
                }
                else if (nodeType.Equals(NodeType.Battle))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[1], spawnPosition, spawnRotation, parentTransform);
                }
                else if (nodeType.Equals(NodeType.Boss))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[2], spawnPosition, spawnRotation, parentTransform);
                }
                else if (nodeType.Equals(NodeType.Recovery))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[3], spawnPosition, spawnRotation, parentTransform);
                }
                else if (nodeType.Equals(NodeType.Resource))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[4], spawnPosition, spawnRotation, parentTransform);
                }
                else if (nodeType.Equals(NodeType.Start))
                {
                    nodeData.nodeOnType = Instantiate(nodeContainer.node[5], spawnPosition, spawnRotation, parentTransform);
                }
                else
                {
                    Debug.Log("This is unMatched....it is big isue.");
                }
                nodeData.type = nodeType;
            }
        }
        
    }
}