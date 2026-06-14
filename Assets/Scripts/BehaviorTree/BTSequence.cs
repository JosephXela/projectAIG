using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{
    protected List<BTNode> nodes = new List<BTNode>();
    public BTSequence(List<BTNode> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    nodeState_ = NodeState.FAILURE;
                    break;
                default:
                    break;
            }
        }
        nodeState_ = isAnyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return nodeState_;
    }
}
