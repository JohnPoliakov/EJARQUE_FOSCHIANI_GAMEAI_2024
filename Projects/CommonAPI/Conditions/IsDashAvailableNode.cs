using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Conditions
{
    public class IsDashAvailableNode : ConditionNode
    {

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return playerInfo.IsDashAvailable;
        }

    }
}
