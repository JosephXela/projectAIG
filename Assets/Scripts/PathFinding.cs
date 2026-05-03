using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    GridBlock gridRef;

    public Transform startPos;
    public Transform targetPos;

    private void Awake()
    {
        gridRef = GetComponent<GridBlock>();
    }

    private void Update()
    {
        if (startPos != null && targetPos != null)
        {
            FindPath(startPos.position, targetPos.position);
        }
    }

    public void FindPath(Vector3 startPosInp, Vector3 targetPosInp)
    {
        Node startNode = gridRef.NodeFromWorldPoint(startPosInp);
        Node targetNode = gridRef.NodeFromWorldPoint(targetPosInp);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].TotalCost < currentNode.TotalCost ||
                   (openList[i].TotalCost == currentNode.TotalCost &&
                    openList[i].heuristicCost < currentNode.heuristicCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                GetFinalPath(startNode, targetNode);
                return;
            }

            foreach (Node neighborNode in gridRef.GetNeighboringNodes(currentNode))
            {
                if (neighborNode.isWall || closedList.Contains(neighborNode))
                    continue;

                int newCost = currentNode.moveCost + GetManhattanDistance(currentNode, neighborNode);

                if (newCost < neighborNode.moveCost || !openList.Contains(neighborNode))
                {
                    neighborNode.moveCost = newCost;
                    neighborNode.heuristicCost = GetManhattanDistance(neighborNode, targetNode);
                    neighborNode.parentNode = currentNode;

                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }
    }

    void GetFinalPath(Node startNode, Node endNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        finalPath.Reverse();
        gridRef.finalPath = finalPath;
    }

    int GetManhattanDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return dx + dy;
    }
}