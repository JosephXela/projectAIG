using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int thief = ThiefManager.totalThief;
            int capturedThief = PlayerManager.totalCaptured;
            int escapedThief = ThiefManager.totalEscaped;
            if (capturedThief == thief)
            {
                SceneController.instance.NextLevel();
            }else if (escapedThief > 0)
            {
                PlayerManager.Instance.RestartScene();
            }
            else
            {
                PlayerManager.Instance.RestartScene();
            }
        }
    }
}
