using System.Collections.Generic;
using UnityEngine;

public class CollectMoneyNode : BTNode
{
    private ThiefCashController thief;

    public CollectMoneyNode(ThiefCashController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.CashTarget == null ||
            !thief.CashTarget.gameObject.activeInHierarchy)
        {
            thief.CurrentPath = null;
            thief.PathIndex = 0;
            return NodeState.FAILURE;
        }

        bool pathInvalid =
            thief.CurrentPath == null ||
            thief.CurrentPath.Count == 0 ||
            thief.PathIndex >= thief.CurrentPath.Count;

        if (pathInvalid)
        {
            List<Node> newPath = thief.Pathfinding.FindPath(
                thief.SelfTransform.position,
                thief.CashTarget.position);

            if (newPath == null || newPath.Count == 0)
                return NodeState.FAILURE;

            thief.CurrentPath = newPath;
            thief.PathIndex = 0;
        }

        return NodeState.RUNNING;
    }
}