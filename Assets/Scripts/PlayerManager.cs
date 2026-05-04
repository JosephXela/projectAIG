using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public static int totalCaptured = 0;


    void Awake()
    {
        Instance = this;
    }


    public void AddCaptured()
    {
        totalCaptured++;
        Debug.Log("Thief captured: " + totalCaptured);
    }

    public void RestartScene()
    {
        Debug.Log("RESTART!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
