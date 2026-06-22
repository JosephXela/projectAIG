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

    [Header("Sensors")]
    public SightSensor sightSensor;
    public HearingSensor hearingSensor;

    [HideInInspector]
    public bool heardPolice;

    [HideInInspector] public List<Node> currentPath;
    [HideInInspector] public int pathIndex;

    [HideInInspector] public Pathfinding pathfinding;

    // Arah gerak terakhir, dipakai sebagai "facing direction" untuk SightSensor
    private Vector2 lastMoveDir = Vector2.right;

    private BTNode topNode;

    private void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();

        if (sightSensor == null)
            sightSensor = GetComponent<SightSensor>();
        if (hearingSensor == null)
            hearingSensor = GetComponent<HearingSensor>();

        ThiefManager.Instance.RegisterThief();

        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        // PoliceVisibleNode sekarang dibaca dari hasil SightSensor,
        // bukan menghitung jarak/LOS sendiri. Lihat versi baru PoliceVisibleNode.
        PoliceVisibleNode policeVisible =
            new PoliceVisibleNode(this, fleeRange);

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
        UpdateSensorReadings();

        topNode.Evaluate();

        FollowPath();
    }

    private void UpdateSensorReadings()
    {
        // Sight sensor butuh tahu arah hadap thief untuk FOV check.
        if (sightSensor != null)
        {
            sightSensor.facingDirection = lastMoveDir;
        }

        // heardPolice diisi dari HearingSensor, dipakai node lain jika perlu
        // (misal untuk Wander yang lebih waspada, atau debug).
        if (hearingSensor != null)
        {
            heardPolice = hearingSensor.targetHeard;
        }
    }

    public void UpdateLastMoveDir(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
            lastMoveDir = dir;
    }

    public bool IsPoliceSensed()
    {
        // True kalau kelihatan (sight) ATAU kedengaran (hearing).
        bool seen = sightSensor != null && sightSensor.targetSensed;
        bool heard = hearingSensor != null && hearingSensor.targetHeard;
        return seen || heard;
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

        if (finalDir.sqrMagnitude > 0.0001f)
            lastMoveDir = finalDir;

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