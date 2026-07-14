using UnityEngine;
public class HearPoliceNode : BTNode
{
    private ThiefController thief;
    private float hearingRange;
    private Rigidbody2D policeRb;

    public HearPoliceNode(ThiefController thief, float hearingRange)
    {
        this.thief = thief;
        this.hearingRange = hearingRange;

        if (thief.Police != null)
            policeRb = thief.Police.GetComponent<Rigidbody2D>();
    }

    public override NodeState Evaluate()
    {
        if (thief.Police == null || policeRb == null)
            return NodeState.FAILURE;

        float distance = Vector2.Distance(
            thief.SelfTransform.position,
            thief.Police.position);

        float speed = policeRb.linearVelocity.magnitude;

        if (distance <= hearingRange && speed > 0.1f)
            return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }
}