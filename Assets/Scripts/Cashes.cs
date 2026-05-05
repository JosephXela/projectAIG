using UnityEngine;

public class Cashes : MonoBehaviour
{
    //setiap object cash yang ada, "daftarkan" untuk mengetahui total object yang ada
    void Start()
    {
        CashManager.Instance.RegisterCash();
    }
}
