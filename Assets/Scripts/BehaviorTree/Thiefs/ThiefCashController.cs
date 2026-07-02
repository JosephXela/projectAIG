using UnityEngine;

// Interface tambahan khusus untuk thief yang punya cash mechanic.
// Dipisah dari IThiefController supaya BTBasicThief tidak dipaksa
// implement method yang tidak relevan untuknya.
public interface ThiefCashController : ThiefController
{
    bool GoToExit { get; }   // semua cash di scene sudah diambil?
    Transform CashTarget { get; }   // cash yang sedang dituju
}