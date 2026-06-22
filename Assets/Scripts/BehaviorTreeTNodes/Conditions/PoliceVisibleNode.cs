using UnityEngine;

public class PoliceVisibleNode : BTNode
{
    private BTBasicThief thief;
    private float fleeRange;
    private bool isFleeing;

    // Jarak tambahan sebelum thief berhenti flee (hysteresis),
    // mencegah flicker saat police pas di garis batas fleeRange.
    private const float exitBuffer = 1.5f;

    public PoliceVisibleNode(BTBasicThief thief, float fleeRange)
    {
        this.thief = thief;
        this.fleeRange = fleeRange;
    }

    public override NodeState Evaluate()
    {
        if (thief.police == null)
            return NodeState.FAILURE;

        // Sensor adalah sumber kebenaran utama: police harus
        // benar-benar tersensor (vision ATAU hearing) dulu.
        bool sensed = thief.IsPoliceSensed();

        if (!sensed)
        {
            isFleeing = false;
            return NodeState.FAILURE;
        }

        float distance =
            Vector3.Distance(thief.police.position, thief.transform.position);

        if (isFleeing)
        {
            isFleeing = distance <= fleeRange + exitBuffer;
        }
        else
        {
            isFleeing = distance <= fleeRange;
        }

        return isFleeing ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}