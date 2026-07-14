using UnityEngine;
using System.Collections.Generic;

public class BTBasicThief : MonoBehaviour, ThiefController
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

    [HideInInspector] public bool heardPolice;
    [HideInInspector] public List<Node> currentPath;
    [HideInInspector] public int pathIndex;
    [HideInInspector] public Pathfinding pathfinding;

    private Vector2 lastMoveDir = Vector2.right;
    private BTNode topNode;

    public Transform SelfTransform => transform;
    public Transform Police => police;
    public Transform ExitTarget => exitTarget;
    public List<Node> CurrentPath { get => currentPath; set => currentPath = value; }
    public int PathIndex { get => pathIndex; set => pathIndex = value; }
    public Pathfinding Pathfinding => pathfinding;
    public float MoveSpeed => moveSpeed;
    public float FleeSpeedMultiplier => fleeSpeedMultiplier;

    public bool IsPoliceSensed()
    {
        bool seen = sightSensor != null && sightSensor.targetSensed;
        bool heard = hearingSensor != null && hearingSensor.targetHeard;
        return seen || heard;
    }

    public void UpdateLastMoveDir(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
            lastMoveDir = dir;
    }

    public Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength = 0.7f, int rayCount = 32)
    {
        Vector3 bestDir = desiredDir;
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * desiredDir;

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, dir, rayLength,
                LayerMask.GetMask("Obstacle"));

            if (hit.collider != null) continue;

            float score = Vector3.Dot(dir, desiredDir);
            if (score > bestScore) { bestScore = score; bestDir = dir; }
        }

        return bestDir.normalized;
    }

    private void Start()
    {
        pathfinding = FindFirstObjectByType<Pathfinding>();

        if (sightSensor == null) sightSensor = GetComponent<SightSensor>();
        if (hearingSensor == null) hearingSensor = GetComponent<HearingSensor>();

        ThiefManager.Instance.RegisterThief();
        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        var policeVisible = new PoliceVisibleNode(this, fleeRange);
        var exitNearby = new ExitNearbyNode(this, detectionRange);
        var fleeNode = new FleeNode(this);
        var escapeNode = new EscapeNode(this);
        var wanderNode = new WanderNode(this);

        var fleeSequence = new BTSequence(new List<BTNode>
            { policeVisible, fleeNode });

        var escapeSequence = new BTSequence(new List<BTNode>
            { exitNearby, escapeNode });

        topNode = new BTSelector(new List<BTNode>
            { fleeSequence, escapeSequence, wanderNode });
    }

    private void Update()
    {
        UpdateSensorReadings();
        topNode.Evaluate();
        FollowPath();
    }

    private void UpdateSensorReadings()
    {
        if (sightSensor != null) sightSensor.facingDirection = lastMoveDir;
        if (hearingSensor != null) heardPolice = hearingSensor.targetHeard;
    }

    public void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        Vector3 targetPos = currentPath[pathIndex].worldPosition;
        Vector3 desiredDir = (targetPos - transform.position).normalized;
        Vector3 finalDir = GetAvoidDirection(desiredDir, 0.3f, 8);

        transform.position += finalDir * moveSpeed * Time.deltaTime;

        if (finalDir.sqrMagnitude > 0.0001f)
            lastMoveDir = finalDir;

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            pathIndex++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            ThiefManager.Instance.AddEscape();
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        if (currentPath == null) return;
        Gizmos.color = Color.black;
        foreach (Node n in currentPath)
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f);
    }
}