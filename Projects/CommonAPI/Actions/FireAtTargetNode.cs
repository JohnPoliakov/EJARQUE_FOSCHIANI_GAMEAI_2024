using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class FireAtTargetNode : ActionNode
    {

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionFire());
            return true; // Success
        }
    }
}
