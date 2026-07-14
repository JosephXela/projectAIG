using UnityEngine;
using System.Collections.Generic;

public class BTThiefWithCash : MonoBehaviour, ThiefCashController
{
    [Header("References")]
    public Transform exitTarget;
    public Transform police;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float fleeSpeedMultiplier = 2f;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float cashDetecRange = 5f;
    public float fleeRange = 2f;

    [Header("Sensors")]
    public SightSensor sightSensor;
    public HearingSensor hearingSensor;

    [HideInInspector] public bool heardPolice;
    [HideInInspector] public Transform cashTarget;
    [HideInInspector] public Transform lastCashTarget;


    private float cashSearchCooldown = 0.3f;
    private float cashSearchTimer = 0f;

    [HideInInspector] public List<Node> currentPath;
    [HideInInspector] public int pathIndex;
    [HideInInspector] public Pathfinding pathfinding;

    private Vector2 lastMoveDir = Vector2.right;
    private BTNode topNode;

    // --- IThiefController implementation ---
    public Transform SelfTransform => transform;
    public Transform Police => police;
    public Transform ExitTarget => exitTarget;
    public List<Node> CurrentPath { get => currentPath; set => currentPath = value; }
    public int PathIndex { get => pathIndex; set => pathIndex = value; }
    public Pathfinding Pathfinding => pathfinding;
    public float MoveSpeed => moveSpeed;
    public float FleeSpeedMultiplier => fleeSpeedMultiplier;
    public bool GoToExit => ThiefManager.allCashCollected;
    public Transform CashTarget => cashTarget;

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
    // --- End IThiefController ---

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
        // Shared nodes — menerima IThiefController, bukan class konkret
        var policeVisible = new PoliceVisibleNode(this, fleeRange);
        var exitNearby = new ExitNearbyNode(this, detectionRange);
        var fleeNode = new FleeNode(this);
        var escapeNode = new EscapeNode(this);
        var wanderNode = new WanderNode(this);

        // Cash-specific nodes — menerima BTThiefWithCash langsung (wajar,
        // karena node ini memang spesifik untuk thief jenis ini)
        var hasAllCash = new HasMoneyNode(this);
        var cashNearby = new MoneyNearbyNode(this);
        var collectCash = new CollectMoneyNode(this);

        var fleeSequence = new BTSequence(new List<BTNode>
            { policeVisible, fleeNode });

        var collectSequence = new BTSequence(new List<BTNode>
            { new BTInverter(hasAllCash), cashNearby, collectCash });

        var escapeSequence = new BTSequence(new List<BTNode>
            { hasAllCash, exitNearby, escapeNode });

        topNode = new BTSelector(new List<BTNode>
            { fleeSequence, collectSequence, escapeSequence, wanderNode });
    }

    private void Update()
    {
        UpdateSensorReadings();
        UpdateCashTarget();
        topNode.Evaluate();
        FollowPath();
    }

    private void UpdateSensorReadings()
    {
        if (sightSensor != null) sightSensor.facingDirection = lastMoveDir;
        if (hearingSensor != null) heardPolice = hearingSensor.targetHeard;
    }

    public void UpdateCashTarget()
    {
        cashSearchTimer -= Time.deltaTime;
        if (cashSearchTimer > 0f) return;

        cashSearchTimer = cashSearchCooldown;

        if (cashTarget == null || !cashTarget.gameObject.activeInHierarchy)
        {
            cashTarget = GetClosestCash();
            lastCashTarget = null;
        }
    }

    public Transform GetClosestCash()
    {
        GameObject[] allCash = GameObject.FindGameObjectsWithTag("Cash");

        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject cash in allCash)
        {
            if (cash == null) continue;

            Vector2 toCash = (Vector2)cash.transform.position - (Vector2)transform.position;
            float dist = toCash.magnitude;
            if (dist > cashDetecRange) continue;

            if (sightSensor != null)
            {
                float angleToTarget = Vector2.Angle(lastMoveDir, toCash.normalized);
                if (angleToTarget > sightSensor.angle / 2f) continue;

                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position, toCash.normalized, dist, sightSensor.obstacleMask);
                if (hit.collider != null) continue;
            }

            List<Node> testPath = pathfinding.FindPath(
                transform.position, cash.transform.position);

            if (testPath == null || testPath.Count == 0) continue;

            if (dist < minDist) { minDist = dist; nearest = cash.transform; }
        }

        return nearest;
    }

    public void OnCashCollected()
    {
        currentPath = null;
        pathIndex = 0;
        cashTarget = null;
        lastCashTarget = null;
        cashSearchTimer = 0f;
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

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            pathIndex++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit") && GoToExit)
        {
            ThiefManager.Instance.AddEscape();
            Destroy(gameObject);
        }

        if (other.CompareTag("Cash"))
        {
            ThiefManager.Instance.AddCash();
            Destroy(other.gameObject);
            OnCashCollected();

            Debug.Log("Cash stolen: " + ThiefManager.totalCash + "/" + CashManager.cashes);
            Debug.Log("Go to exit: " + GoToExit);
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