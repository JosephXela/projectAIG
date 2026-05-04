using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int thief = ThiefManager.totalThief;
            int capturedThief = PlayerManager.totalCaptured;
            if (capturedThief == thief)
            {
                SceneController.instance.NextLevel();
            }
            else
            {
                PlayerManager.Instance.RestartScene();
            }
        }
    }
}
