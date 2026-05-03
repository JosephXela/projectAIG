using UnityEngine;

public class BasicThief : MonoBehaviour
{
    public float moveSpeed = 2f;

    Rigidbody2D rb;
    Vector2 moveVec = Vector2.zero;

    public float wanderCooldown = 2.5f;
    float timer;

    float lastX = 0;
    float lastY = 0;

    // obstacle avoidance
    public float avoidDistance = 1.5f;
    public float avoidForce = 8f;
    public LayerMask wallLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = wanderCooldown;
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Wander();
            timer = wanderCooldown;
        }

        Move();
    }

    // =========================
    // WANDER (smooth random)
    // =========================
    void Wander()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        moveVec = Vector2.Lerp(moveVec, dir, 0.5f);

        lastX = moveVec.x;
        lastY = moveVec.y;
    }

    // =========================
    // AVOID WALL (16 RAY - 360 degrees)
    // =========================
    Vector2 AvoidWall()
    {
        int rayCount = 16; // jumlah ray (semakin banyak semakin halus)
        float angleStep = 360f / rayCount;

        Vector2 avoid = Vector2.zero;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dir,
                avoidDistance,
                wallLayer
            );

            if (hit.collider != null)
            {
                // semakin dekat semakin kuat dorongannya
                float strength = (avoidDistance - hit.distance) / avoidDistance;
                avoid += hit.normal * strength * avoidForce;
            }
        }
        return avoid;
    }

    Vector2 CheckRay(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            avoidDistance,
            wallLayer
        );

        if (hit.collider != null)
        {
            return hit.normal * avoidForce;
        }

        return Vector2.zero;
    }

    // =========================
    // MOVEMENT
    // =========================
    void Move()
    {
        Vector2 desired = moveVec * moveSpeed;

        // bias supaya tidak muter
        desired += rb.linearVelocity.normalized * 0.5f;

        // avoidance tidak terlalu dominan
        desired += AvoidWall() * 0.3f;

        rb.linearVelocity = Vector2.SmoothDamp(
            rb.linearVelocity,
            desired,
            ref moveVec,
            Time.fixedDeltaTime * 6f
        );

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, moveSpeed);

        if (rb.linearVelocity.magnitude < 0.2f)
        {
            moveVec = Random.insideUnitCircle.normalized;
        }
    }

    // =========================
    // SEEK
    // =========================
    void Seek(Transform target, float speed)
    {
        Vector2 dir =
            (target.position - transform.position).normalized;

        moveVec = dir;

        moveSpeed = speed;
    }

    // =========================
    // FLEE
    // =========================
    void Flee(Transform target, float speed)
    {
        Vector2 dir =
            (transform.position - target.position).normalized;

        moveVec = dir;

        moveSpeed = speed;
    }
    
    // =========================
    // PATH FINDING
    // =========================
    void PathFinding()
    {

    }
}