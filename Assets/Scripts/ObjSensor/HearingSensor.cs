using System.Collections;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    [Header("Hearing Settings")]
    public float radius = 6f;

    [Header("Detection")]
    public Transform targetGO;
    public LayerMask targetMask;

    public bool targetHeard;

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
        // Hearing: tidak butuh FOV, tidak butuh line-of-sight.
        // Cukup cek apakah target berada dalam radius dengar.
        Collider2D[] rangeChecks =
            Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            targetGO = rangeChecks[0].transform;
            targetHeard = true;
        }
        else
        {
            targetHeard = false;
        }
    }
}