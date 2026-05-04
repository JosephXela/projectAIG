using System.Collections.Generic;
using UnityEngine;

public class ThiefCash : MonoBehaviour
{
    public Transform exitTarget;
    public Transform police;
    Transform cashTarget;

    private Transform lastCashTarget;
    private ThiefState previousState;

    public float moveSpeed = 3f;
    public float fleeSpeedMultiplier = 2f;

    private List<Node> currentPath;
    private int pathIndex;

    private Pathfinding pathfinding;

    public enum ThiefState
    {
        Seek,
        Flee,
        Wander,
        SeekCash
    }

    public ThiefState currentState;

    public float detectionRange = 5f;
    public float cashDetecRange = 5f;
    public float fleeRange = 2f;

    public bool goToExit = false;


    void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();
        ThiefManager.Instance.RegisterThief();
    }
    Transform GetClosestCash()
    {
        GameObject[] allCash = GameObject.FindGameObjectsWithTag("Cash");
        Debug.Log("All cash: " + allCash.Length);

        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject cash in allCash)
        {
            if (cash == null) continue;

            float dist = Vector2.Distance(transform.position, cash.transform.position);

            // Optional: skip cash yang terlalu jauh dari detection range
            if (dist > cashDetecRange) continue;

            List<Node> testPath = pathfinding.FindPath(transform.position, cash.transform.position);

            if (testPath == null || testPath.Count == 0)
            {
                Debug.LogWarning("Cash tidak reachable: " + cash.name);
                continue;
            }

            if (dist < minDist)
            {
                minDist = dist;
                nearest = cash.transform;
            }
        }

        Debug.Log("Nearest reachable cash: " + nearest);
        return nearest;
    }

    void Update()
    {
        float distToPolice = Vector3.Distance(transform.position, police.position);
        float distToExit = Vector3.Distance(transform.position, exitTarget.position);

        cashTarget = GetClosestCash();

        float distToCash = Mathf.Infinity;

        if (cashTarget != null)
        {
            distToCash = Vector2.Distance(transform.position, cashTarget.position);
        } 

        if (goToExit)
        {
            currentState = ThiefState.Seek;
        }
        else
        {
            if (distToPolice < fleeRange)
            {
                currentState = ThiefState.Flee;
            }
            else if (cashTarget != null && distToCash < cashDetecRange)
            {
                currentState = ThiefState.SeekCash;
            }
            else if (distToExit < detectionRange)
            {
                currentState = ThiefState.Seek;
            }
            else
            {
                currentState = ThiefState.Wander;
            }
        }

        if (currentState != previousState)
        {
            currentPath = null;
            pathIndex = 0;
            previousState = currentState;
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

            case ThiefState.SeekCash:
                SeekCash();
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

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            pathIndex++;
        }
    }

    // =========================
    // SEEK (KE EXIT)
    // =========================
    void SeekExit()
    {
        if (exitTarget == null)
        {
            Debug.LogWarning("Exit target belum di-assign!");
            return;
        }

        if (pathfinding == null)
        {
            Debug.LogWarning("Pathfinding tidak ditemukan!");
            return;
        }

        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            currentPath = pathfinding.FindPath(transform.position, exitTarget.position);
            pathIndex = 0;

            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning("Path ke exit gagal dibuat! Posisi thief: "
                + transform.position + " | Posisi exitTarget: " + exitTarget.position
                + " | Exit name: " + exitTarget.name);
            }
            else
            {
                Debug.Log("Path ke exit berhasil. Jumlah node: " + currentPath.Count);
            }
        }
    }

    // =========================
    // SEEK (KE CASH)
    // =========================
    void SeekCash()
    {
        if (cashTarget == null)
        {
            currentState = ThiefState.Wander;
            currentPath = null;
            pathIndex = 0;
            return;
        }

        if (
            currentPath == null ||
            currentPath.Count == 0 ||
            pathIndex >= currentPath.Count ||
            lastCashTarget != cashTarget
        )
        {
            currentPath = pathfinding.FindPath(transform.position, cashTarget.position);
            pathIndex = 0;
            lastCashTarget = cashTarget;

            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning("Path ke cash gagal dibuat: " + cashTarget.name);

                cashTarget = null;
                lastCashTarget = null;
                currentState = ThiefState.Wander;
                return;
            }
            else
            {
                Debug.Log("Path ke cash berhasil. Jumlah node: " + currentPath.Count);
            }
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
        if (other.CompareTag("Exit")&& goToExit)
        {
            ThiefManager.Instance.AddEscape();
            Destroy(gameObject);
        }
        if (other.CompareTag("Cash"))
        {
            ThiefManager.Instance.AddCash();

            Destroy(other.gameObject);

            currentPath = null;
            pathIndex = 0;
            cashTarget = null;
            lastCashTarget = null;

            goToExit = ThiefManager.totalCash >= CashManager.cashes;

            Debug.Log("Cash stolen: " + ThiefManager.totalCash + "/" + CashManager.cashes);
            Debug.Log("Go to exit: " + goToExit);
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
