using UnityEngine;
using System.Collections.Generic;
public interface ThiefController
{
    Transform SelfTransform { get; }
    Transform Police { get; }
    Transform ExitTarget { get; }

    List<Node> CurrentPath { get; set; }
    int PathIndex { get; set; }
    Pathfinding Pathfinding { get; }

    float MoveSpeed { get; }
    float FleeSpeedMultiplier { get; }

    bool IsPoliceSensed();

    Vector3 GetAvoidDirection(Vector3 desiredDir, float rayLength, int rayCount);
    void UpdateLastMoveDir(Vector3 dir);
}