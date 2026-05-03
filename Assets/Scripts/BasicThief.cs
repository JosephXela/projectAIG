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
    // AVOID WALL (5 RAY)
    // =========================
    Vector2 AvoidWall()
    {
        Vector2 forward = rb.linearVelocity.magnitude > 0.1f
            ? rb.linearVelocity.normalized
            : moveVec.normalized;

        float[] angles = { -60f, -30f, 0f, 30f, 60f };

        Vector2 avoid = Vector2.zero;

        foreach (float angle in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;
            avoid += CheckRay(dir);
        }

        // anti-stuck (kalau mentok depan)
        RaycastHit2D frontHit = Physics2D.Raycast(
            transform.position,
            forward,
            avoidDistance,
            wallLayer
        );

        if (frontHit.collider != null)
        {
            Vector2 side = Vector2.Perpendicular(frontHit.normal);
            avoid += side * avoidForce;
        }

        // sedikit randomness biar tidak kaku
        avoid += Random.insideUnitCircle * 0.2f;

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

}