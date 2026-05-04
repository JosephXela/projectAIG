using UnityEngine;

public class CashManager : MonoBehaviour
{
    public static CashManager Instance;
    public static int cashes = 0;
    private void Awake()
    {
        Instance = this;
    }
    public void RegisterCash()
    {
        cashes++;
        Debug.Log("Total Cash: " + cashes);
    }
}
