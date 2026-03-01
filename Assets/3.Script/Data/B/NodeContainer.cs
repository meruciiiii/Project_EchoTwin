using System.Collections.Generic;
using UnityEngine;
public class NodeContainer : MonoBehaviour
{
    public Floors[] floors;
    public GameObject[] node;
#if UNITY_EDITOR
    [ContextMenu("NodeContainer/FloorAraay Create")]
    public void FloorCount()
    {
        floors = new Floors[6];
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i] = new Floors();
        }
        floors[0].Floor = new NodeData[1];
        floors[0].Floor[0] = new NodeData();
        floors[1].Floor = new NodeData[2];
        floors[1].Floor[0] = new NodeData();
        floors[1].Floor[1] = new NodeData();
        floors[2].Floor = new NodeData[3];
        floors[2].Floor[0] = new NodeData();
        floors[2].Floor[1] = new NodeData();
        floors[2].Floor[2] = new NodeData();
        floors[3].Floor = new NodeData[4];
        floors[3].Floor[0] = new NodeData();
        floors[3].Floor[1] = new NodeData();
        floors[3].Floor[2] = new NodeData();
        floors[3].Floor[3] = new NodeData();
        floors[4].Floor = new NodeData[4];
        floors[4].Floor[0] = new NodeData();
        floors[4].Floor[1] = new NodeData();
        floors[4].Floor[2] = new NodeData();
        floors[4].Floor[3] = new NodeData();
        floors[5].Floor = new NodeData[1];
        floors[5].Floor[0] = new NodeData();
    }
    [ContextMenu("NodeContainer/NodeArray Create")]
    public void NodeArray()
    {
        node = new GameObject[6];
    }
#endif
}
[System.Serializable]
public class Floors
{
    public NodeData[] Floor;
}
[System.Serializable]
public class NodeData
{
    public GameObject positionTarget;
    public NodeType type;
    public GameObject nodeOnType;
    public bool clear;
    public List<StageNode> exploreNode;
}
