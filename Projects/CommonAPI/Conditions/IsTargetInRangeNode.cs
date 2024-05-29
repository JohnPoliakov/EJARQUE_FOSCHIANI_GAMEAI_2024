using AI_BehaviorTree_AIGameUtility;
using System;
using UnityEngine;

namespace CommonAPI.Conditions
{
    public class IsTargetInRangeNode : ConditionNode
    {
        private Func<Vector3> target;
        private float range;

        public IsTargetInRangeNode(Func<Vector3> target,float range)
        {
            this.range = range;
            this.target = target;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return Vector3.Distance(playerInfo.Transform.Position, target.Invoke()) <= range;
        }

    }
}
