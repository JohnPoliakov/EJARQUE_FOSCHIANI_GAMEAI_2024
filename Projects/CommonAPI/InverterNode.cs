using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using Windows.Services.Maps;

namespace CommonAPI
{
    // Class that revert the expected result
    public class InverterNode : ActionNode
    {
        private ActionNode child;

        public InverterNode(ActionNode child)
        {
            this.child = child;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            return !child.Execute(playerInfo, actionList);
        }

    }
}
