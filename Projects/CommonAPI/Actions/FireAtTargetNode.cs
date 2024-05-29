using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class FireAtTargetNode : ActionNode
    {
        private Func<PlayerInformations> target;

        public FireAtTargetNode(Func<PlayerInformations> target)
        {
            this.target = target;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {

            if (target == null || target.Invoke().BonusOnPlayer[EBonusType.Invulnerability] > 0)
            {

                actionList.Add(new AIActionReload());
            }
            else
            {
                actionList.Add(new AIActionFire());
            }
            
            return true; // Success
        }
    }
}
