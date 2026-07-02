using UnityEngine;
using System.Collections.Generic;

// Interface ini adalah "kontrak" yang harus dipenuhi oleh semua
// MonoBehaviour yang ingin dikendalikan oleh node-node BT.
// BTBasicThief dan BTThiefWithCash keduanya implement interface ini,
// sehingga PoliceVisibleNode, FleeNode, WanderNode, EscapeNode, ExitNearbyNode
// bisa dipakai ulang tanpa perubahan apapun.
public interface ThiefController
{
    // --- Referensi objek ---
    Transform SelfTransform { get; }
    Transform Police { get; }
    Transform ExitTarget { get; }

    // --- State movement ---
    List<Node> CurrentPath { get; set; }
    int PathIndex { get; set; }
    Pathfinding Pathfinding { get; }

    // --- Parameter movement ---
    float MoveSpeed { get; }
    float FleeSpeedMultiplier { get; }

    // --- Sensor ---
    bool IsPoliceSensed();

    // --- Helper movement ---
    Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength, int rayCount);
    void UpdateLastMoveDir(Vector3 dir);
}