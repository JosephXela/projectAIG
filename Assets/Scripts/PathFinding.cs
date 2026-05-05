using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    GridBlock gridRef;

    private void Awake()
    {
        gridRef = GetComponent<GridBlock>(); //referensi gridblock
    }

    public List<Node> FindPath(Vector3 startPosInp, Vector3 targetPosInp)
    {
        //dictionary untuk menyimpan NodeData tiap node
        //dibuat saat dibutuhkan, bukan sekaligus untuk semua node (hemat memori)
        Dictionary<Node, NodeData> nodeDataMap = new Dictionary<Node, NodeData>();

        //helper lokal — ambil NodeData dari node, buat baru kalau belum ada
        NodeData GetData(Node node)
        {
            if (!nodeDataMap.ContainsKey(node))
                nodeDataMap[node] = new NodeData(node);

            return nodeDataMap[node];
        }

        //konversi posisi dunia - node grid terdekat
        Node startNode = gridRef.NodeFromWorldPoint(startPosInp);
        Node targetNode = gridRef.NodeFromWorldPoint(targetPosInp);

        //ambil NodeData untuk start dan target
        NodeData startData = GetData(startNode);
        NodeData targetData = GetData(targetNode);

        //openList  = node yang belum dievaluasi, kandidat berikutnya
        //closedList = node yang sudah dievaluasi, tidak perlu diproses lagi
        List<NodeData> openList = new List<NodeData>();
        HashSet<NodeData> closedList = new HashSet<NodeData>();

        //mulai dari node start
        openList.Add(startData);

        //loop sampai semua kandidat habis atau path ketemu
        while (openList.Count > 0)
        {
            //cari node dengan fCost terkecil di openList
            //kalau fCost sama, pilih yang hCost-nya lebih kecil (lebih dekat ke tujuan)
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

            //pindahkan current dari openList ke closedList
            //(sudah dipilih, tidak perlu dievaluasi lagi)
            openList.Remove(current);
            closedList.Add(current);

            //kalau current adalah target - path ketemu, traceback lewat parent
            if (current.node == targetNode)
            {
                return RetracePath(startData, current);
            }

            //evaluasi semua tetangga node current (4 arah)
            foreach (Node neighbor in gridRef.GetNeighboringNodes(current.node))
            {
                if (neighbor.isWall) continue; //skip jika wall

                NodeData neighborData = GetData(neighbor);

                if (closedList.Contains(neighborData)) //skip jika tetangga sudah dievaluasi
                    continue;

                int newCost = current.gCost + GetDistance(current.node, neighbor); //hitung gCost baru = gCost current + jarak ke tetangga

                //update tetangga kalau:
                // - newCost lebih kecil dari gCost sebelumnya (ketemu jalur lebih pendek)
                // - atau tetangga belum pernah masuk openList sama sekali
                if (newCost < neighborData.gCost || !openList.Contains(neighborData))
                {
                    neighborData.gCost = newCost;
                    neighborData.hCost = GetDistance(neighbor, targetNode);
                    neighborData.parent = current;

                    //tambah ke openList kalau belum ada
                    if (!openList.Contains(neighborData))
                        openList.Add(neighborData);
                }
            }
        }
        //openList habis tapi target tidak ketemu - tidak ada path (terhalang wall)
        return null;
    }

    List<Node> RetracePath(NodeData start, NodeData end)
    {
        List<Node> path = new List<Node>(); //buat list path
        NodeData current = end; //mulai dari end (node tujuan)

        //telurusi node dari tujuan sampai awal
        //tambah node current ke path, lalu loncat ke parentnya
        while (current != start)
        {
            path.Add(current.node);
            current = current.parent;
        }
        
        path.Reverse(); //balik urutan
        return path;
    }

    int GetDistance(Node a, Node b)
    {
        //menghitung gCost(jarak dari start) dan hCost(estimasi jarak ke target)
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return dx + dy;
    }
}