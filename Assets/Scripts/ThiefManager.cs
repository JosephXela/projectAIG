using UnityEngine;

public class ThiefManager : MonoBehaviour
{
    public static ThiefManager Instance;

    public static int totalThief = 0;
    public static int totalEscaped = 0;
    public static int totalCash = 0;
    public static bool allCashCollected = false;

    private void Awake()
    {
        Instance = this;
    }

    //method untuk "daftarkan" object thief
    public void RegisterThief()
    {
        totalThief++;
        Debug.Log("Total Thief: " + totalThief);
    }
    //method untuk "hitung" object thief yang keluar
    public void AddEscape()
    {
        totalEscaped++;
        Debug.Log("Thief escaped: " + totalEscaped);
    }
    //method untuk "hitung" object cash yang ditangkap thief
    public void AddCash()
    {
        totalCash++;
        allCashCollected = totalCash >= CashManager.cashes;
        Debug.Log("Cash Collected: " + totalCash);
    }
}