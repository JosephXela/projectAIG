public class MoneyNearbyNode : BTNode
{
    private ThiefCashController thief;

    public MoneyNearbyNode(ThiefCashController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        if (thief.CashTarget != null &&
            thief.CashTarget.gameObject.activeInHierarchy)
            return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }
}