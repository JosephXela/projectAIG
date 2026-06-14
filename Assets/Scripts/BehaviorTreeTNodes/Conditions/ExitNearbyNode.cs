using UnityEngine;

public class ExitNearbyNode : BTNode
{
    private Transform exitTarget;
    private Transform thief;
    private float range;

    public ExitNearbyNode(
        Transform exitTarget,
        Transform thief,
        float range)
    {
        this.exitTarget = exitTarget;
        this.thief = thief;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        float distance =
            Vector3.Distance(
                exitTarget.position,
                thief.position);

        return distance <= range
            ? NodeState.SUCCESS
            : NodeState.FAILURE;
    }
}