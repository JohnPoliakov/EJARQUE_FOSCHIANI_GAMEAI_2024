
using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI
{
    public abstract class ActionNode
    {

        public abstract bool Execute(PlayerInformations playerInfo, List<AIAction> actionList);
    }
}
