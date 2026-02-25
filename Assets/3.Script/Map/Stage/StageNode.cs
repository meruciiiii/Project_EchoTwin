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
}
public enum NodeType
{
    Battle, Resource, Altar, Recovery
}