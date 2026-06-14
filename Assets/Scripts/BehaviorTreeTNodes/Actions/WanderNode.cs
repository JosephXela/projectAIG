using UnityEngine;

public class WanderNode : BTNode
{
    private BTBasicThief thief;

    public WanderNode(BTBasicThief thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.currentPath == null ||
            thief.currentPath.Count == 0 ||
            thief.pathIndex >= thief.currentPath.Count)
        {
            GridBlock grid =
                Object.FindFirstObjectByType<GridBlock>();

            Node randomNode =
                grid.GetRandomWalkableNode(
                    thief.transform.position,
                    4f);

            if (randomNode != null)
            {
                thief.currentPath =
                    thief.pathfinding.FindPath(
                        thief.transform.position,
                        randomNode.worldPosition);

                thief.pathIndex = 0;
            }
        }

        return NodeState.RUNNING;
    }
}