using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CommonAPI.Actions
{
    public class LookAtTargetNode : ActionNode
    {
        private Func<Vector3> targetPosition;

        public LookAtTargetNode(Func<Vector3> targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionLookAtPosition(targetPosition.Invoke() + new Vector3(0, 0.2f, 0)));
            return true; // Success
        }
    }
}
