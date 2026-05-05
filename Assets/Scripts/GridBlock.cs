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
        nodeDiameter = nodeRadius * 2; //hitung diameter node
        //hitung jumlah node di grid
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid(); //buat grid
    }

    void CreateGrid()
    {
        //buat array 2D node
        nodeGrid = new Node[gridSizeX, gridSizeY];

        //cari titik pojok kiri bawah
        Vector3 bottomLeft = transform.position
            - Vector3.right * gridWorldSize.x / 2
            - Vector3.up * gridWorldSize.y / 2;

        //loop setiap kolom dan baris, di grid dari kiri bawah ke kanan atas.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //hitung posisi dunia tiap node
                //dari bottomLeft, geser kanan sebanyak x node lalu tambah nodeRadius supaya posisi tepat di tengah node, bukan di tepi
                Vector3 worldPoint = bottomLeft
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.up * (y * nodeDiameter + nodeRadius);

                //cek apakah node kena obstacle, OverlapBox cek apakah ada collider berlayer wallMask (Obstacle) di area node tersebut)
                bool isWall = Physics2D.OverlapBox(
                    worldPoint,
                    Vector2.one * nodeDiameter * 0.8f,
                    0,
                    wallMask
                );

                //simpan node ke array
                nodeGrid[x, y] = new Node(isWall, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighboringNodes(Node currentNode)
    {
        List<Node> neighbors = new List<Node>(); //list kosong

        //loop 3×3 di sekitar node, 9 kombinasi offset (x,y)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; //skip node sendiri (0,0)

                if (Mathf.Abs(x) == Mathf.Abs(y)) continue; //skip diagonal (4 arah saja)

                //hitung koordinat tetangga
                int checkX = currentNode.gridX + x;
                int checkY = currentNode.gridY + y;

                //validasi dalam batas grid
                //pastikan koordinat tidak keluar dari array — node di tepi grid tidak punya tetangga di luar batas.
                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(nodeGrid[checkX, checkY]);
                }
            }
        }

        return neighbors; //kembalikan list tetangga yang valid ke A* untuk dievaluasi.
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        //konversi posisi dunia ke nilai 0-1
        float xPos = (worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float yPos = (worldPos.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;

        //paksa nilai tetap di antara 0.0 - 1.0, kalau posisi dunia di luar batas grid, tetap snap ke node terdekat di tepi
        xPos = Mathf.Clamp01(xPos);
        yPos = Mathf.Clamp01(yPos);

        //konversi 0-1 ke indeks array
        int x = Mathf.RoundToInt((gridSizeX - 1) * xPos);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPos);

        return nodeGrid[x, y]; //return node
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
        List<Node> candidates = new List<Node>(); //list kandidat (semua node yang memenuhi syarat)

        //filter node yang valid
        foreach (Node n in nodeGrid)
        {
            //syarat 1 : node bisa diinjak (bukan obstacle)
            //syarat 2 : node dalam jangkauan radius dari posisi thief
            if (!n.isWall &&
                Vector3.Distance(n.worldPosition, position) <= radius)
            {
                candidates.Add(n);
            }
        }

        //jika tidak ada kandidat
        if (candidates.Count == 0)
            return null;

        //pilih random dari kandidat
        return candidates[Random.Range(0, candidates.Count)];
    }
}