using UnityEngine;

public class FleeNode : BTNode
{
    private BTBasicThief thief;

    public FleeNode(BTBasicThief thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.police == null)
            return NodeState.FAILURE;

        Vector3 desiredDir =
            (thief.transform.position -
             thief.police.position)
            .normalized;

        Vector3 finalDir =
            thief.GetAvoidDirection(
                desiredDir,
                1.2f,
                16);

        float fleeSpeed =
            thief.moveSpeed *
            thief.fleeSpeedMultiplier;

        thief.transform.position +=
            finalDir *
            fleeSpeed *
            Time.deltaTime;

        thief.currentPath = null;

        return NodeState.RUNNING;
    }
}