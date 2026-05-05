using UnityEngine;
using System.Collections.Generic;

public class BasicThief : MonoBehaviour
{
    public Transform exitTarget;
    public Transform police;

    public float moveSpeed = 3f;
    public float fleeSpeedMultiplier = 2f;

    private List<Node> currentPath;
    private int pathIndex;

    private Pathfinding pathfinding;

    private ThiefState previousState;

    public enum ThiefState
    {
        Seek,
        Flee,
        Wander
    }

    public ThiefState currentState;

    public float detectionRange = 5f;
    public float fleeRange = 2f;

    void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();
        ThiefManager.Instance.RegisterThief();
    }

    void Update()
    {

        ThiefState nextState = ChooseState();

        HandleState(nextState);

        MyState();

        FollowPath();
    }
    ThiefState ChooseState ()
    {
        float distToPolice = Vector3.Distance(transform.position, police.position);
        float distToExit = Vector3.Distance(transform.position, exitTarget.position);


        if (distToPolice < fleeRange)
        {
            return ThiefState.Flee;
        }
        else if (distToExit < detectionRange)
        {
            return ThiefState.Seek;
        }
        else
        {
            return ThiefState.Wander;
        }
    }
    void HandleState(ThiefState nextState)
    {
        //setiap state hanya reset path sekali setiap frame (reset saat state benar-benar berganti)
        if (nextState != previousState)
        {
            currentState = nextState;
            currentPath = null;
            pathIndex = 0;
            previousState = currentState;
        }
        else
        {
            currentState = nextState;
        }
    }
    void MyState()
    {
        switch (currentState)
        {
            case ThiefState.Seek:
                SeekExit();
                break;

            case ThiefState.Flee:
                Flee();
                break;

            case ThiefState.Wander:
                Wander();
                break;
        }
    }
    void FollowPath()
    {
        if (currentState == ThiefState.Flee) return; //tidak mengikuti path
        if (currentPath == null || currentPath.Count == 0) return; //path belum dibuat/kosong, tidak bisa diikuti
        if (pathIndex >= currentPath.Count) return; //kalau semua node sudah dilewati (sudah sampai tujuan)

        Vector3 targetPos = currentPath[pathIndex].worldPosition; //ambil node yang sedang dituju dari list of nodes

        //tambah steering di sini
        Vector3 desiredDir = (targetPos - transform.position).normalized;
        Vector3 finalDir = GetAvoidDirection(desiredDir, 0.3f, 8); // rayLength pendek, rayCount sedikit

        transform.position += finalDir * moveSpeed * Time.deltaTime;

        //cek apakah sudah sampai node, jika node kurang dari 0.1f, anggap sudah sampai, naikkan pathIndex agar frame berikutnya menuju node selanjutnya
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }
    void SeekExit()
    {
        if (exitTarget == null) return; //cek exit sudah diassign apa belum

        //kalau path belum siap atau sudah habis, hitung path dari posisi thief ke exit, reset pathIndex (node mulai dari node pertama)
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            currentPath = pathfinding.FindPath(transform.position, exitTarget.position);
            pathIndex = 0;
        }
    }
    void Flee()
    {
        if (police == null) return; //cek (player) sudah diassign apa belum

        Vector3 desiredDir = (transform.position - police.position).normalized; //arah yang diinginkan (posisi thief - posisi player)
        Vector3 finalDir = GetAvoidDirection(desiredDir, 1.2f, 16); //menjauhi wall, tidak kena wall

        float fleeSpeed = moveSpeed * fleeSpeedMultiplier; //kecepatan flee lebih cepat dari kecepatan normal (steering)
        transform.position += finalDir * fleeSpeed * Time.deltaTime;

        currentPath = null; //menghapus path
    }
    void Wander()
    {
        //jika path null atau kosong, generate path baru ke titik random
        if (currentPath == null || currentPath.Count == 0)
        {
            GenerateNewWanderPath();
            return;
        }
        
        //jika path sudah habis dilalui, semua node sudah dilewati, generate path baru
        if (pathIndex >= currentPath.Count)
        {
            GenerateNewWanderPath();
        }
    }
    void GenerateNewWanderPath()
    {
        GridBlock grid = FindFirstObjectByType<GridBlock>(); //ambil referensi grid
        Node randomNode = grid.GetRandomWalkableNode(transform.position, 4f); //mencari random node yang walkable dalam radius 4f dari posisi thief sekarang

        //buath path ke random node tersebut, bisa null
        if (randomNode != null)
        {
            currentPath = pathfinding.FindPath(transform.position, randomNode.worldPosition);
            pathIndex = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            ThiefManager.Instance.AddEscape();
            Destroy(gameObject);

        }
    }
    void OnDrawGizmos()
    {
        if (currentPath == null) return; //tidak ada path, tidak ada gambar

        Gizmos.color = Color.black;
        foreach (Node n in currentPath)
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f); //ukuran kubus 0.3 * 0.3 * 0.3
    }
    Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength = 0.7f, int rayCount = 32) 
    {
        //desiredDir = arah yang diinginkan (menjauhi player/menuju node), rayLength = seberapa jauh raycast mendeteksi obstacle, rayCount = jumlah arah

        Vector3 bestDir = desiredDir; //asusmi arah terbaik
        float bestScore = -Mathf.Infinity; //nilai paling kecil, supaya arah apapun yang valid pasti menang di perbandingan pertama. 

        //hitung sudut tiap ray
        //bagi 360° secara merata sebanyak rayCount, setiap ray dirotasi dari desiredDir, bukan dari sumbu tetap.
        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * desiredDir;

            //tembak ray dari posisi thief ke arah dir sejauh rayLength, hanya deteksi layer Obstacle
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dir,
                rayLength,
                LayerMask.GetMask("Obstacle")
            );

            //skip arah yang kena obstacle
            if (hit.collider != null) continue;

            //pilih arah terbaik
            //dari semua arah yang tidak kena obstacle, pilih yang dot product-nya paling tinggi = paling mirip dengan arah asli yang diinginkan
            //dot = 1 (lurus), dot = 0 (belok 90 derajat), dot = -1 (balik arah)
            float score = Vector3.Dot(dir, desiredDir);
            if (score > bestScore)
            {
                bestScore = score;
                bestDir = dir;
            }
        }
        return bestDir.normalized;
    }
}