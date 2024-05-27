using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI
{
    public abstract class ConditionNode : ActionNode
    {
        public abstract bool Evaluate(PlayerInformations playerInfo);

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            return Evaluate(playerInfo);
        }
    }
}
