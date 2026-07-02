using UnityEngine;

public class EscapeNode : BTNode
{
    private ThiefController thief;

    public EscapeNode(ThiefController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.ExitTarget == null)
            return NodeState.FAILURE;

        if (thief.CurrentPath == null ||
            thief.CurrentPath.Count == 0 ||
            thief.PathIndex >= thief.CurrentPath.Count)
        {
            thief.CurrentPath =
                thief.Pathfinding.FindPath(
                    thief.SelfTransform.position,
                    thief.ExitTarget.position);

            thief.PathIndex = 0;
        }

        return NodeState.RUNNING;
    }
}