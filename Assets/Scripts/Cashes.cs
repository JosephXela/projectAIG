using UnityEngine;

public class Cashes : MonoBehaviour
{
    void Start()
    {
        CashManager.Instance.RegisterCash();
    }
}
