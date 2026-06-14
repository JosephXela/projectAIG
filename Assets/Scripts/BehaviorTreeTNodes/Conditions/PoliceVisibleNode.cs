using UnityEngine;

public class PoliceVisibleNode : BTNode
{
    private Transform police;
    private Transform thief;
    private float range;

    public PoliceVisibleNode(
        Transform police,
        Transform thief,
        float range)
    {
        this.police = police;
        this.thief = thief;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        float distance =
            Vector3.Distance(
                police.position,
                thief.position);

        return distance <= range
            ? NodeState.SUCCESS
            : NodeState.FAILURE;
    }
}