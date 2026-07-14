using UnityEngine;

// Dipasang pada prefab "sound noise" (object kosong / kecil di layer "Target")
// Prefab ini di-spawn oleh NoiseEmitter setiap kali player membuat suara,
// lalu menghancurkan dirinya sendiri setelah beberapa saat.
//
// HearingSensor akan mendeteksi objek ini (bukan Player secara langsung)
// melalui OverlapCircleAll, sehingga hearing benar-benar merespon "suara",
// bukan sekadar "kehadiran" player.
public class Noise : MonoBehaviour
{
    [Tooltip("Berapa lama noise signal ini bertahan sebelum hilang. " +
             "Sebaiknya >= interval SensorCheck (0.2f) agar sempat terdeteksi minimal sekali.")]
    public float lifetime = 1f;

    // Opsional: dipakai HearingSensor untuk tahu seberapa "keras" suaranya,
    // misal untuk logika lanjutan (mis. AI jadi lebih waspada kalau noise besar)
    [HideInInspector] public float loudness;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}