using System.Collections.Generic;
public class StageNode
{
    public StageNode()
    {
        nodeType = NodeType.Battle;
        nextNodes = new HashSet<StageNode>();
    }
    public NodeType nodeType;
    public int floorIndex;
    public int nodeIndex;

    public HashSet<StageNode> nextNodes;

    public bool isCleared = false;
    public bool isReachable = false;
}
public enum NodeType
{
    Altar, Battle, Boss, Resource, Recovery, Clear
}