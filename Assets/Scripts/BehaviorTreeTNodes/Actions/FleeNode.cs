using UnityEngine;

public class FleeNode : BTNode
{
    private ThiefController thief;

    public FleeNode(ThiefController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.Police == null)
            return NodeState.FAILURE;

        Vector3 desiredDir =
            (thief.SelfTransform.position -
             thief.Police.position)
            .normalized;

        Vector3 finalDir =
            thief.GetAvoidDirection(
                desiredDir,
                1.2f,
                16);

        float fleeSpeed =
            thief.MoveSpeed *
            thief.FleeSpeedMultiplier;

        thief.SelfTransform.position +=
            finalDir *
            fleeSpeed *
            Time.deltaTime;

        thief.UpdateLastMoveDir(finalDir);

        thief.CurrentPath = null;
        thief.PathIndex = 0;

        return NodeState.RUNNING;
    }
}