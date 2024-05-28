using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class StopMovementNode : ActionNode
    {

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionStopMovement());
            return true; // Success

        }
    }
}
