using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI.TreeBehaviour
{
    public class SequenceNode : ControlNode
    {

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            foreach (ActionNode child in children)
            {
                if (!child.Execute(playerInfo, actionList))
                    return false; // Failure
            }
            return true; // Success
        }
    }
}
