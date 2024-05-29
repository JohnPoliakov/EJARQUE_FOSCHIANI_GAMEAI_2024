using AI_BehaviorTree_AIGameUtility;
using System;

namespace CommonAPI.Conditions
{
    public class IsSeekingBonusNode : ConditionNode
    {

        private Func<BonusInformations> bonus;

        public IsSeekingBonusNode(Func<BonusInformations> bonus)
        {
            this.bonus = bonus;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return bonus != null;
        }

    }
}
