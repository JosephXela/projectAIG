using UnityEngine;
using System.Collections.Generic;

public class BTBasicThief : MonoBehaviour
{
    [Header("References")]
    public Transform exitTarget;
    public Transform police;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float fleeSpeedMultiplier = 2f;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float fleeRange = 2f;

    [HideInInspector] public List<Node> currentPath;
    [HideInInspector] public int pathIndex;

    [HideInInspector] public Pathfinding pathfinding;

    private BTNode topNode;

    private void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();

        ThiefManager.Instance.RegisterThief();

        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        PoliceVisibleNode policeVisible =
            new PoliceVisibleNode(
                police,
                transform,
                fleeRange);

        ExitNearbyNode exitNearby =
            new ExitNearbyNode(
                exitTarget,
                transform,
                detectionRange);

        FleeNode fleeNode =
            new FleeNode(this);

        EscapeNode escapeNode =
            new EscapeNode(this);

        WanderNode wanderNode =
            new WanderNode(this);

        BTSequence fleeSequence =
            new BTSequence(
                new List<BTNode>()
                {
                    policeVisible,
                    fleeNode
                });

        BTSequence escapeSequence =
            new BTSequence(
                new List<BTNode>()
                {
                    exitNearby,
                    escapeNode
                });

        topNode =
            new BTSelector(
                new List<BTNode>()
                {
                    fleeSequence,
                    escapeSequence,
                    wanderNode
                });
    }

    private void Update()
    {
        topNode.Evaluate();

        FollowPath();
    }

    public void FollowPath()
    {
        if (currentPath == null ||
            currentPath.Count == 0)
            return;

        if (pathIndex >= currentPath.Count)
            return;

        Vector3 targetPos =
            currentPath[pathIndex].worldPosition;

        Vector3 desiredDir =
            (targetPos - transform.position)
            .normalized;

        Vector3 finalDir =
            GetAvoidDirection(
                desiredDir,
                0.3f,
                8);

        transform.position +=
            finalDir *
            moveSpeed *
            Time.deltaTime;

        if (Vector3.Distance(
            transform.position,
            targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }

    public Vector3 GetAvoidDirection(
        Vector3 desiredDir,
        float rayLength = 0.7f,
        int rayCount = 32)
    {
        Vector3 bestDir = desiredDir;
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angle =
                (360f / rayCount) * i;

            Vector3 dir =
                Quaternion.Euler(0, 0, angle)
                * desiredDir;

            RaycastHit2D hit =
                Physics2D.Raycast(
                    transform.position,
                    dir,
                    rayLength,
                    LayerMask.GetMask("Obstacle"));

            if (hit.collider != null)
                continue;

            float score =
                Vector3.Dot(dir, desiredDir);

            if (score > bestScore)
            {
                bestScore = score;
                bestDir = dir;
            }
        }

        return bestDir.normalized;
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