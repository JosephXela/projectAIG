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
        float distToPolice = Vector3.Distance(transform.position, police.position);
        float distToExit = Vector3.Distance(transform.position, exitTarget.position);

        if (distToPolice < fleeRange)
        {
            currentState = ThiefState.Flee;
        }
        else if (distToExit < detectionRange)
        {
            currentState = ThiefState.Seek;
        }
        else
        {
            currentState = ThiefState.Wander;
        }

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

        FollowPath();
    }

    // =========================
    // PATH FOLLOW
    // =========================
    void FollowPath()
    {
        if (currentState == ThiefState.Flee) return;

        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        Vector3 targetPos = currentPath[pathIndex].worldPosition;

        Vector3 desiredDir = (targetPos - transform.position).normalized;

        Vector3 finalDir = GetAvoidDirection(desiredDir);

        transform.position += finalDir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            pathIndex++;
        }
    }

    // =========================
    // SEEK (KE EXIT)
    // =========================
    void SeekExit()
    {
        if (exitTarget == null) return;

        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            currentPath = pathfinding.FindPath(transform.position, exitTarget.position);
            pathIndex = 0;
        }
    }

    // =========================
    // FLEE (STEERING, TANPA A*)
    // =========================
    void Flee()
    {
        if (police == null) return;

        Vector3 desiredDir = (transform.position - police.position).normalized;

        Vector3 finalDir = GetAvoidDirection(desiredDir, 1.2f, 16);

        float fleeSpeed = moveSpeed * fleeSpeedMultiplier;

        transform.position += finalDir * fleeSpeed * Time.deltaTime;

        currentPath = null;
    }

    // =========================
    // WANDER (SELESAIKAN PATH DULU)
    // =========================
    void Wander()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            GenerateNewWanderPath();
            return;
        }

        if (pathIndex >= currentPath.Count)
        {
            GenerateNewWanderPath();
        }
    }

    void GenerateNewWanderPath()
    {
        GridBlock grid = FindFirstObjectByType<GridBlock>();

        Node randomNode = grid.GetRandomWalkableNode(transform.position, 4f);

        if (randomNode != null)
        {
            currentPath = pathfinding.FindPath(transform.position, randomNode.worldPosition);
            pathIndex = 0;
        }
    }

    // =========================
    // EXIT DETECTION
    // =========================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            ThiefManager.Instance.AddEscape();
            Destroy(gameObject);
        }
    }

    // =========================
    // DEBUG PATH
    // =========================
    void OnDrawGizmos()
    {
        if (currentPath == null) return;

        Gizmos.color = Color.black;

        foreach (Node n in currentPath)
        {
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f);
        }
    }

    Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength = 0.7f, int rayCount = 32)
    {
        Vector3 bestDir = desiredDir;
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * desiredDir;

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dir,
                rayLength,
                LayerMask.GetMask("Obstacle")
            );

            if (hit.collider != null)
                continue;

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