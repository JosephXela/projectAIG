using UnityEngine;

public class WanderNode : BTNode
{
    private ThiefController thief;

    public WanderNode(ThiefController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.CurrentPath == null ||
            thief.CurrentPath.Count == 0 ||
            thief.PathIndex >= thief.CurrentPath.Count)
        {
            GridBlock grid =
                Object.FindFirstObjectByType<GridBlock>();

            Node randomNode =
                grid.GetRandomWalkableNode(
                    thief.SelfTransform.position,
                    4f);

            if (randomNode != null)
            {
                thief.CurrentPath =
                    thief.Pathfinding.FindPath(
                        thief.SelfTransform.position,
                        randomNode.worldPosition);
                thief.PathIndex = 0;
            }
        }

        return NodeState.RUNNING;
    }
}