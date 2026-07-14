using UnityEngine;

// Dipasang pada Player.
// Bertugas men-spawn NoiseSignal prefab (layer "Target") setiap kali player
// bergerak, dengan radius/frekuensi berbeda tergantung status gerakan.
//
// Kalau player diam total, tidak ada noise yang di-spawn sama sekali,
// sehingga HearingSensor musuh otomatis tidak akan mendeteksi apa-apa
// -- tanpa perlu logika tambahan di sisi sensor.
public class SpawnNoise : MonoBehaviour
{
    [Header("Noise Prefab")]
    public GameObject noiseSignalPrefab; // prefab kosong + NoiseSignal.cs, layer = Target

    [Header("Emit Interval")]
    [Tooltip("Jeda antar spawn noise saat berjalan")]
    public float walkEmitInterval = 0.5f;
    [Tooltip("Jeda antar spawn noise saat berlari (lebih sering = lebih 'berisik')")]
    public float sprintEmitInterval = 0.2f;

    [Header("Movement Threshold")]
    [Tooltip("Kecepatan minimum agar dianggap 'bergerak' (menghasilkan suara)")]
    public float moveSpeedThreshold = 0.1f;
    [Tooltip("Kecepatan minimum agar dianggap 'berlari'")]
    public float sprintSpeedThreshold = 4f;

    private Rigidbody2D rb;
    private float emitTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;

        // Player diam -> tidak menghasilkan suara sama sekali
        if (speed < moveSpeedThreshold)
        {
            emitTimer = 0f;
            return;
        }

        bool isSprinting = speed >= sprintSpeedThreshold;
        float interval = isSprinting ? sprintEmitInterval : walkEmitInterval;

        emitTimer += Time.deltaTime;
        if (emitTimer >= interval)
        {
            emitTimer = 0f;
            EmitNoise();
        }
    }

    private void EmitNoise()
    {
        if (noiseSignalPrefab == null) return;

        GameObject go = Instantiate(noiseSignalPrefab, transform.position, Quaternion.identity);
        // Pastikan layer prefab sesuai target mask HearingSensor (biasanya sudah diset di prefab-nya,
        // baris ini cuma jaga-jaga)
        go.layer = noiseSignalPrefab.layer;
    }
}