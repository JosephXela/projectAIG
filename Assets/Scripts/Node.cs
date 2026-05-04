using UnityEngine;
public class Node
{
    public int gridX;
    public int gridY;

    public bool isWall;
    public Vector3 worldPosition;

    public Node(bool isWall, Vector3 worldPosition, int gridX, int gridY)
    {
        this.isWall = isWall;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}