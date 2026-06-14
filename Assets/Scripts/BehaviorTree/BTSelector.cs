using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTNode
{
    protected List<BTNode> nodes = new List<BTNode>();
    public BTSelector(List<BTNode> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    nodeState_ = NodeState.RUNNING;
                    return nodeState_;
                case NodeState.SUCCESS:
                    nodeState_ = NodeState.SUCCESS;
                    return nodeState_;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        nodeState_ = NodeState.FAILURE;
        return nodeState_;
    }
}
