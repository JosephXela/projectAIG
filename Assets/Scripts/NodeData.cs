using UnityEngine;

public class NodeData
{
    public Node node;
    public int gCost;
    public int hCost;
    public NodeData parent;

    public int fCost => gCost + hCost;

    public NodeData(Node node)
    {
        this.node = node;
    }
}
