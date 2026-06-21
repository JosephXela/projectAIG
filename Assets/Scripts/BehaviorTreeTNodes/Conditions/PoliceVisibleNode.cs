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

    //public override NodeState Evaluate()
    //{
    //    float distance =
    //        Vector3.Distance(
    //            police.position,
    //            thief.position);

    //    return distance <= range
    //        ? NodeState.SUCCESS
    //        : NodeState.FAILURE;
    //}

    //dibawah untuk sight cone
    public override NodeState Evaluate()
    {
        float distance =
            Vector3.Distance(
                police.position,
                thief.position);

        if (distance > range)
            return NodeState.FAILURE;

        Vector3 dirToPolice =
            (police.position - thief.position).normalized;

        float angle =
            Vector3.Angle(
                thief.up,
                dirToPolice);

        return angle <= 45f
            ? NodeState.SUCCESS
            : NodeState.FAILURE;
    }
}