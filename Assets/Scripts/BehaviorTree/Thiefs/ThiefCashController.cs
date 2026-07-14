using UnityEngine;

public interface ThiefCashController : ThiefController
{
    bool GoToExit { get; }
    Transform CashTarget { get; }
}