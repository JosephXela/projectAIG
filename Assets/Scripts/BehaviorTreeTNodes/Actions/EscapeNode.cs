using UnityEngine;

public class EscapeNode : BTNode
{
    private BTBasicThief thief;

    public EscapeNode(BTBasicThief thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.exitTarget == null)
            return NodeState.FAILURE;

        if (thief.currentPath == null ||
            thief.currentPath.Count == 0 ||
            thief.pathIndex >= thief.currentPath.Count)
        {
            thief.currentPath =
                thief.pathfinding.FindPath(
                    thief.transform.position,
                    thief.exitTarget.position);

            thief.pathIndex = 0;
        }

        return NodeState.RUNNING;
    }
}