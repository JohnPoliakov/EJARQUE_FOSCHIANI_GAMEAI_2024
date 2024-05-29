using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CommonAPI.Actions
{
    public class MoveToTargetNode : ActionNode
    {
        private Func<Vector3> targetPosition;

        public MoveToTargetNode(Func<Vector3> targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionMoveToDestination(targetPosition.Invoke()));
            return true; // Success
        }
    }
}
