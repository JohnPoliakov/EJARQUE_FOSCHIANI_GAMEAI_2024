using AI_BehaviorTree_AIGameUtility;

namespace CommonAPI.Conditions
{
    public class IsDashAvailableNode : ConditionNode
    {

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return playerInfo.IsDashAvailable;
        }

    }
}
