using UnityEngine;
using System.Collections.Generic;

public class GridBlock : MonoBehaviour
{
    public LayerMask wallMask;
    public Vector2 gridWorldSize;

    public float nodeRadius;
    float nodeDiameter;

    public float distanceBetweenNodes;

    public Node[,] nodeGrid;

    public int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        nodeGrid = new Node[gridSizeX, gridSizeY];

        Vector3 bottomLeft = transform.position
            - Vector3.right * gridWorldSize.x / 2
            - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.up * (y * nodeDiameter + nodeRadius);

                bool isWall = Physics2D.OverlapBox(
                    worldPoint,
                    Vector2.one * nodeDiameter * 0.8f,
                    0,
                    wallMask
                );

                nodeGrid[x, y] = new Node(isWall, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighboringNodes(Node currentNode)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; //skip node sendiri

                if (Mathf.Abs(x) == Mathf.Abs(y)) continue; //skip diagonal (4 arah saja)

                int checkX = currentNode.gridX + x;
                int checkY = currentNode.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(nodeGrid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float xPos = (worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float yPos = (worldPos.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;

        xPos = Mathf.Clamp01(xPos);
        yPos = Mathf.Clamp01(yPos);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPos);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPos);

        return nodeGrid[x, y];
    }

    private void OnDrawGizmos()
    {
        if (nodeGrid == null) return;

        foreach (Node n in nodeGrid)
        {
            if (n.isWall)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.white;

            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - distanceBetweenNodes));
        }
    }

    public Node GetRandomWalkableNode(Vector3 position, float radius)
    {
        List<Node> candidates = new List<Node>();

        foreach (Node n in nodeGrid)
        {
            if (!n.isWall &&
                Vector3.Distance(n.worldPosition, position) <= radius)
            {
                candidates.Add(n);
            }
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }
}