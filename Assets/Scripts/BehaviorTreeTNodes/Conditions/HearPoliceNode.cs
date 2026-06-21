using UnityEngine;
//pencuri bisa mendengar hanya jika polisi bergerak
public class HearPoliceNode : BTNode
{
    private BTBasicThief thief;
    private Transform police;
    private float hearingRange;
    private Rigidbody2D policeRb;

    public HearPoliceNode(
       BTBasicThief thief,
       Transform police,
       float hearingRange)
    {
        this.thief = thief;
        this.police = police;
        this.hearingRange = hearingRange;

        if (police != null)
        {
            policeRb = police.GetComponent<Rigidbody2D>();
        }
        
    }
    public override NodeState Evaluate()
    {
        if (police == null || policeRb == null)
            return NodeState.FAILURE;

        float distance = Vector2.Distance(
            thief.transform.position,
            police.position);

        float speed = policeRb.linearVelocity.magnitude;

        if (distance <= hearingRange && speed > 0.1f)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
