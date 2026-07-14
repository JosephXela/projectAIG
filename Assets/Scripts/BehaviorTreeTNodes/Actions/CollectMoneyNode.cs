using System.Collections.Generic;
using UnityEngine;

public class CollectMoneyNode : BTNode
{
    private ThiefCashController thief;
    private Transform trackedCashTarget; // cash yang sedang di-tuju oleh path saat ini

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
            trackedCashTarget = null;
            return NodeState.FAILURE;
        }

        bool pathInvalid =
            thief.CurrentPath == null ||
            thief.CurrentPath.Count == 0 ||
            thief.PathIndex >= thief.CurrentPath.Count;

        // Path dianggap invalid juga kalau cash target berubah
        // (misal sebelumnya path menuju wander point, atau cash lain)
        if (!pathInvalid && trackedCashTarget != thief.CashTarget)
        {
            pathInvalid = true;
        }

        if (pathInvalid)
        {
            List<Node> newPath = thief.Pathfinding.FindPath(
                thief.SelfTransform.position,
                thief.CashTarget.position);

            if (newPath == null || newPath.Count == 0)
                return NodeState.FAILURE;

            thief.CurrentPath = newPath;
            thief.PathIndex = 0;
            trackedCashTarget = thief.CashTarget;
        }

        return NodeState.RUNNING;
    }
}