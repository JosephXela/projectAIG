using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int thief = ThiefManager.totalThief; //ambil total object thief yang ada
            int capturedThief = PlayerManager.totalCaptured; //ambil total object thief yang ditangkap player
            int escapedThief = ThiefManager.totalEscaped; //ambil total object thief yang keluar
            if (capturedThief == thief) //jika player menangkap semua thief yang ada
            {
                SceneController.instance.NextLevel();
            }else if (escapedThief > 0) //jika ada thief yang keluar
            {
                PlayerManager.Instance.RestartScene();
            }
            else //jika player tidak menangkap satupun thief
            {
                PlayerManager.Instance.RestartScene();
            }
        }
    }
}
