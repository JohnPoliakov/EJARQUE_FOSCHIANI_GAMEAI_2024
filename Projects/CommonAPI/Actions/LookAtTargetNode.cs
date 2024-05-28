using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Actions
{
    public class LookAtTargetNode : ActionNode
    {
        private Vector3 targetPosition;

        public LookAtTargetNode(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionLookAtPosition(targetPosition));
            return true; // Success
        }
    }
}
