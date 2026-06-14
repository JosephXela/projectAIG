using System.Collections.Generic;
using UnityEngine;

public class BTInverter : BTNode
{
    protected BTNode node;
    public BTInverter(BTNode node)
    {
        this.node = node;
    }
    public override NodeState Evaluate()
    {
        switch (node.Evaluate())
        {
            case NodeState.RUNNING:
                nodeState_ = NodeState.RUNNING;
                break;
            case NodeState.SUCCESS:
                nodeState_ = NodeState.FAILURE;
                break;
            case NodeState.FAILURE:
                nodeState_ = NodeState.SUCCESS;
                break;
            default:
                break;
        }
        return nodeState_;
    }
}
