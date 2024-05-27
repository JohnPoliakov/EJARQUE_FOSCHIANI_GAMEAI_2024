using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI.TreeBehaviour
{
    public class SelectorNode : ControlNode
    {

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            foreach (ActionNode child in children)
            {
                if (child.Execute(playerInfo, actionList))
                    return true; // Success
            }
            return false; // Failure
        }
    }
}
