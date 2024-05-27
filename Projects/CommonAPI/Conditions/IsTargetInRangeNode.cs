using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Conditions
{
    public class IsTargetInRangeNode : ConditionNode
    {
        private PlayerInformations target;
        private float range;

        public IsTargetInRangeNode(PlayerInformations target, float range)
        {
            this.target = target;
            this.range = range;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return Vector3.Distance(playerInfo.Transform.Position, target.Transform.Position) <= range;
        }

    }
}
