using UnityEngine;

public class NodeData
{
    //menyimpan data perhitungan A* untuk node tersebut selama pathfinding berlangsung
    //dipakai sementara saat FindPath() jalan, dibuang setelah path ketemu

    public Node node; //node grid aslinya
    public int gCost;
    public int hCost;
    public NodeData parent; //node sebelumnya di path terbaik (menyimpan "dari mana saya datang")

    public int fCost => gCost + hCost;

    public NodeData(Node node)
    {
        this.node = node;
    }
}
