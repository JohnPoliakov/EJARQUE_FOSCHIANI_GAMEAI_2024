using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI
{
    // Class that revert the expected result
    public class InverterNode : ActionNode
    {
        private ConditionNode child;

        public InverterNode(ConditionNode child)
        {
            this.child = child;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            return !child.Execute(playerInfo, actionList);
        }

    }
}
