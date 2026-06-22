using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisionSensor))]
public class VisionSensorEditor : Editor
{
    public void OnSceneGUI()
    {
        VisionSensor sense = (VisionSensor)target;
        Handles.color = Color.white;

        // Lingkaran radius, normal mengarah ke kamera (sumbu Z) supaya terlihat di plane XY
        Handles.DrawWireArc(
            sense.transform.position,
            Vector3.forward,
            Vector3.right,
            360,
            sense.radius);

        // Arah hadap diambil dari facingDirection (arah gerak terakhir thief),
        // bukan dari rotasi transform, karena VisionSensor 2D top-down
        // umumnya tidak merotasi GameObject-nya.
        float facingAngleDeg =
            Mathf.Atan2(sense.facingDirection.y, sense.facingDirection.x) * Mathf.Rad2Deg;

        Vector3 viewAngle1 = DirectionFromAngle(facingAngleDeg, -sense.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(facingAngleDeg, sense.angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(
            sense.transform.position,
            sense.transform.position + viewAngle1 * sense.radius);
        Handles.DrawLine(
            sense.transform.position,
            sense.transform.position + viewAngle2 * sense.radius);

        if (sense.targetSensed && sense.targetGO != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(sense.transform.position, sense.targetGO.position);
        }
    }

    private Vector3 DirectionFromAngle(float baseAngleDeg, float angleOffsetDeg)
    {
        float angle = baseAngleDeg + angleOffsetDeg;
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0);
    }
}