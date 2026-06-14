using UnityEngine;

[System.Serializable]
public abstract class BTNode
{
    protected NodeState nodeState_;
    public NodeState nodeState
    {
        get { return nodeState_; }
    }
    public abstract NodeState Evaluate();
}
public enum NodeState
{
    RUNNING, SUCCESS, FAILURE,
}
