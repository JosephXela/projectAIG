using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    GridBlock gridRef;

    private void Awake()
    {
        gridRef = GetComponent<GridBlock>();
    }

    public List<Node> FindPath(Vector3 startPosInp, Vector3 targetPosInp)
    {
        Dictionary<Node, NodeData> nodeDataMap = new Dictionary<Node, NodeData>();

        NodeData GetData(Node node)
        {
            if (!nodeDataMap.ContainsKey(node))
                nodeDataMap[node] = new NodeData(node);

            return nodeDataMap[node];
        }

        Node startNode = gridRef.NodeFromWorldPoint(startPosInp);
        Node targetNode = gridRef.NodeFromWorldPoint(targetPosInp);

        NodeData startData = GetData(startNode);
        NodeData targetData = GetData(targetNode);

        List<NodeData> openList = new List<NodeData>();
        HashSet<NodeData> closedList = new HashSet<NodeData>();

        openList.Add(startData);

        while (openList.Count > 0)
        {
            NodeData current = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < current.fCost ||
                   (openList[i].fCost == current.fCost &&
                    openList[i].hCost < current.hCost))
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current.node == targetNode)
            {
                return RetracePath(startData, current);
            }

            foreach (Node neighbor in gridRef.GetNeighboringNodes(current.node))
            {
                if (neighbor.isWall) continue;

                NodeData neighborData = GetData(neighbor);

                if (closedList.Contains(neighborData))
                    continue;

                int newCost = current.gCost + GetDistance(current.node, neighbor);

                if (newCost < neighborData.gCost || !openList.Contains(neighborData))
                {
                    neighborData.gCost = newCost;
                    neighborData.hCost = GetDistance(neighbor, targetNode);
                    neighborData.parent = current;

                    if (!openList.Contains(neighborData))
                        openList.Add(neighborData);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(NodeData start, NodeData end)
    {
        List<Node> path = new List<Node>();
        NodeData current = end;

        while (current != start)
        {
            path.Add(current.node);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return dx + dy;
    }
}