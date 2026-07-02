using UnityEngine;

public class ExitNearbyNode : BTNode
{
    private ThiefController thief;
    private float range;

    public ExitNearbyNode(ThiefController thief, float range)
    {
        this.thief = thief;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        if (thief.ExitTarget == null)
            return NodeState.FAILURE;

        float distance = Vector3.Distance(
            thief.ExitTarget.position,
            thief.SelfTransform.position);

        return distance <= range
            ? NodeState.SUCCESS
            : NodeState.FAILURE;
    }
}