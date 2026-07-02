public class HasMoneyNode : BTNode
{
    private ThiefCashController thief;

    public HasMoneyNode(ThiefCashController thief)
    {
        this.thief = thief;
    }

    public override NodeState Evaluate()
    {
        return thief.GoToExit
            ? NodeState.SUCCESS
            : NodeState.FAILURE;
    }
}