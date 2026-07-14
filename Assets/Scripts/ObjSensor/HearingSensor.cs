using System.Collections;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    [Header("Hearing Settings")]
    public float radius = 6f;
    public float memoryDuration = 1f; // tetap "heard" selama X detik setelah noise terakhir terdeteksi
    private float lastHeardTime = -999f;

    [Header("Detection")]
    public Transform targetGO;
    public LayerMask targetMask;

    public bool targetHeard;

    [Header("Buffer")]
    [Tooltip("Ukuran buffer OverlapCircleNonAlloc, sesuaikan dengan estimasi maksimum noise signal aktif bersamaan")]
    public int maxDetections = 8;
    private Collider2D[] hitBuffer;

    private void Start()
    {
        hitBuffer = new Collider2D[maxDetections];
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
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetMask);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        int count = Physics2D.OverlapCircle(transform.position, radius, filter, hitBuffer);

        if (count > 0)
        {
            Transform nearest = null;
            float nearestDist = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                float dist = Vector2.Distance(transform.position, hitBuffer[i].transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = hitBuffer[i].transform;
                }
            }
            targetGO = nearest;
            lastHeardTime = Time.time;
        }

        targetHeard = (Time.time - lastHeardTime) <= memoryDuration;
    }
}