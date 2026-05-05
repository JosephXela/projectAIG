using UnityEngine;

public class CashManager : MonoBehaviour
{
    public static CashManager Instance;
    public static int cashes = 0;
    private void Awake()
    {
        Instance = this;
    }
    //method untuk "daftarkan" object cash
    public void RegisterCash()
    {
        cashes++;
        Debug.Log("Total Cash: " + cashes);
    }
}
