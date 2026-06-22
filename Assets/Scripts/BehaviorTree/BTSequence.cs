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
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    nodeState_ = NodeState.RUNNING;
                    return nodeState_;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.FAILURE:
                    nodeState_ = NodeState.FAILURE;
                    return nodeState_;
            }
        }

        nodeState_ = NodeState.SUCCESS;
        return nodeState_;
    }
}