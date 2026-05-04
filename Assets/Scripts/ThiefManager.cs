using UnityEngine;

public class ThiefManager : MonoBehaviour
{
    public static ThiefManager Instance;

    public static int totalThief = 0;
    public static int totalEscaped = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterThief()
    {
        totalThief++;
        Debug.Log("Total Thief: " + totalThief);
    }

    public void AddEscape()
    {
        totalEscaped++;
        Debug.Log("Thief escaped: " + totalEscaped);
        if (totalEscaped == totalThief)
        {
            PlayerManager.Instance.RestartScene();
        }
    }
}