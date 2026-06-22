using System.Collections;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    public float radius = 5f;
    [Range(0, 360)]
    public float angle = 90f;

    [Header("Detection")]
    public Transform targetGO;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Facing Direction")]
    // Untuk 2D top-down, arah "menghadap" biasanya dari movement direction,
    // bukan transform.right/up bawaan. Di-set dari luar (misalnya dari BTBasicThief).
    public Vector2 facingDirection = Vector2.right;

    public bool targetSensed;

    private void Start()
    {
        StartCoroutine(SensoryRoutine());
    }

    private IEnumerator SensoryRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            SensorCheck();
        }
    }

    private void SensorCheck()
    {
        Collider2D[] rangeChecks =
            Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            targetGO = rangeChecks[0].transform;

            Vector2 directionToTarget =
                ((Vector2)targetGO.position - (Vector2)transform.position).normalized;

            if (Vector2.Angle(facingDirection, directionToTarget) < angle / 2f)
            {
                float distanceToTarget =
                    Vector2.Distance(transform.position, targetGO.position);

                RaycastHit2D hit =
                    Physics2D.Raycast(
                        transform.position,
                        directionToTarget,
                        distanceToTarget,
                        obstacleMask);

                targetSensed = hit.collider == null;
            }
            else
            {
                targetSensed = false;
            }
        }
        else
        {
            targetSensed = false;
        }
    }
}