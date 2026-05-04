using UnityEngine;

public class ThiefManager : MonoBehaviour
{
    public static ThiefManager Instance;

    public int totalEscaped = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddEscape()
    {
        totalEscaped++;
        Debug.Log("Thief escaped: " + totalEscaped);
    }
}
