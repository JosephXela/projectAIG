using UnityEngine;
using System.Collections.Generic;

public class BasicThief : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3f;

    public float pathUpdateDelay = 0.5f;

    private float timer;

    private List<Node> currentPath;
    private int pathIndex;

    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // update path tiap beberapa detik
        if (timer >= pathUpdateDelay)
        {
            timer = 0;
            UpdatePath();
        }

        FollowPath();
    }

    void UpdatePath()
    {
        if (target == null) return;

        currentPath = pathfinding.FindPath(transform.position, target.position);
        pathIndex = 0;
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        Vector3 targetPos = currentPath[pathIndex].worldPosition;

        // gerak ke node berikutnya
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // kalau sudah dekat, lanjut ke node berikutnya
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }

    // DEBUG PATH (biar keliatan di scene)
    void OnDrawGizmos()
    {
        if (currentPath == null) return;

        Gizmos.color = Color.black;

        foreach (Node n in currentPath)
        {
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f);
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
}