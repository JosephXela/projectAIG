using UnityEngine;

public class PoliceVisibleNode : BTNode
{
    private ThiefController thief;
    private float fleeRange;
    private bool isFleeing;
    private const float exitBuffer = 1.5f;

    public PoliceVisibleNode(ThiefController thief, float fleeRange)
    {
        this.thief = thief;
        this.fleeRange = fleeRange;
    }

    public override NodeState Evaluate()
    {
        if (thief.Police == null)
            return NodeState.FAILURE;

        if (!thief.IsPoliceSensed())
        {
            isFleeing = false;
            return NodeState.FAILURE;
        }

        float distance = Vector3.Distance(
            thief.Police.position,
            thief.SelfTransform.position);

        isFleeing = isFleeing
            ? distance <= fleeRange + exitBuffer
            : distance <= fleeRange;

        return isFleeing ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}