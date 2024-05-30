using AI_BehaviorTree_AIGameUtility;
using UnityEngine;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class DashNodeLouis : ActionNode
    {
        private GameWorldUtils utils;
        private Vector3 targetPosition;
        private float range;

        public DashNodeLouis(GameWorldUtils utils, Vector3 targetPosition, float range)
        {
            this.utils = utils;
            this.targetPosition = targetPosition;
            this.range = range;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            Vector3 direction = ComputeDirection(playerInfo);
            actionList.Add(new AIActionDash(direction));
            return true; // Success
        }

        private Vector3 ComputeDirection(PlayerInformations playerInfo)
        {
            return (targetPosition - playerInfo.Transform.Position).normalized;
        }
    }
}
