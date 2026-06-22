using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HearingSensor))]
public class HearingSensorEditor : Editor
{
    public void OnSceneGUI()
    {
        HearingSensor sense = (HearingSensor)target;

        // Hearing tidak punya FOV, cuma radius bulat penuh.
        Handles.color = Color.cyan;
        Handles.DrawWireArc(
            sense.transform.position,
            Vector3.forward,
            Vector3.right,
            360,
            sense.radius);

        if (sense.targetHeard && sense.targetGO != null)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(sense.transform.position, sense.targetGO.position);
        }
    }
}