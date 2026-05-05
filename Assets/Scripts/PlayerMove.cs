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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void FixedUpdate()
    {
        if (goToExit)
        {
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
    // AVOID WALL (SAMA PERSIS THIEF)
    // =========================
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

    // =========================
    // DEBUG PATH (OPTIONAL)
    // =========================
    void OnDrawGizmos()
    {
        if (currentPath == null) return;

        Gizmos.color = Color.blue;

        foreach (Node n in currentPath)
        {
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f);
        }
    }
}
