using AI_BehaviorTree_AIGameUtility;
using System;

namespace CommonAPI.Conditions
{
    public class HasTargetNode : ConditionNode
    {

        private Func<PlayerInformations> target;

        public HasTargetNode(Func<PlayerInformations> target)
        {
            this.target = target;
        }   

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return target != null;
        }

    }
}
