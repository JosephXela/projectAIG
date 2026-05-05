using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;
    Vector2 moveVec = Vector2.zero;
    Rigidbody2D rb;

    public Transform exitTarget;

    private Pathfinding pathfinding;
    private List<Node> currentPath;
    private int pathIndex;
    public bool goToExit = false;

    public GameObject civilianPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void FixedUpdate()
    {
        if (goToExit)
        {
            //kalau sedang auto-path ke exit, input player diabaikan supaya tidak bertabrakan dengan FollowPath()
            moveSpeed = 2f;
            FollowPath();
        }
        else
        {
            rb.linearVelocity = moveVec * moveSpeed;
        }
    }

    public void OnMove(InputValue input)
    {
        if (goToExit) return;

        Vector2 inputVec = input.Get<Vector2>();
        moveVec = new Vector2(inputVec.x, inputVec.y);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Thief"))
        {
            PlayerManager.Instance.AddCaptured();
            Destroy(other.gameObject);
            int thief = ThiefManager.totalThief;
            int capturedThief = PlayerManager.totalCaptured;
            if (capturedThief == thief)
            {
                currentPath = pathfinding.FindPath(transform.position, exitTarget.position);
                pathIndex = 0;
                goToExit = true;
            }
            else
            {
                goToExit = false;
            }
        }
        else if (other.CompareTag("ThiefHostage"))
        {
            PlayerManager.Instance.AddCaptured();
            Vector3 spawnPos = other.transform.position;
            Destroy(other.gameObject);
            Instantiate(civilianPrefab, spawnPos, Quaternion.identity);
            int thief = ThiefManager.totalThief;
            int capturedThief = PlayerManager.totalCaptured;
            if (capturedThief == thief)
            {
                currentPath = pathfinding.FindPath(transform.position, exitTarget.position);
                pathIndex = 0;
                goToExit = true;
            }
            else
            {
                goToExit = false;
            }
        }
    }
    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return; //path belum dibuat/kosong, tidak bisa diikuti
        if (pathIndex >= currentPath.Count) return; //kalau semua node sudah dilewati (sudah sampai tujuan)

        Vector3 targetPos = currentPath[pathIndex].worldPosition;
        
        //tambah steering di sini
        Vector3 desiredDir = (targetPos - transform.position).normalized;
        Vector3 finalDir = GetAvoidDirection(desiredDir); // rayLength pendek, rayCount sedikit

        transform.position += finalDir * moveSpeed * Time.deltaTime;

        //cek apakah sudah sampai node, jika node kurang dari 0.1f, anggap sudah sampai, naikkan pathIndex agar frame berikutnya menuju node selanjutnya
        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            pathIndex++;
        }
    }
    Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength = 0.7f, int rayCount = 32)
    {
        //desiredDir = arah yang diinginkan (menjauhi player/menuju node), rayLength = seberapa jauh raycast mendeteksi obstacle, rayCount = jumlah arah
        Vector3 bestDir = desiredDir; //asusmi arah terbaik
        float bestScore = -Mathf.Infinity; //nilai paling kecil, supaya arah apapun yang valid pasti menang di perbandingan pertama

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
            if (hit.collider != null)
                continue;

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
    void OnDrawGizmos()
    {
        if (currentPath == null) return; //tidak ada path, tidak ada gambar

        Gizmos.color = Color.blue;

        foreach (Node n in currentPath)
        {
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f); //ukuran kubus 0.3 * 0.3 * 0.3
        }
    }
}
