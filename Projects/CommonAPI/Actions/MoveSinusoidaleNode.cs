using AI_BehaviorTree_AIGameUtility;
using CommonAPI.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Actions
{
    public class MoveSinusoidalNode : ActionNode
    {
        private System.Func<Vector3> targetPositionFunc;
        private float amplitude;
        private float frequency;
        private float elapsedTime;

        public MoveSinusoidalNode(System.Func<Vector3> targetPositionFunc, float amplitude, float frequency)
        {
            this.targetPositionFunc = targetPositionFunc;
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.elapsedTime = 0f;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            elapsedTime += Time.deltaTime;

            Vector3 targetPosition = targetPositionFunc();
            Vector3 directionToTarget = (targetPosition - playerInfo.Transform.Position).normalized;
            Vector3 perpendicularDirection = Vector3.Cross(directionToTarget, Vector3.up).normalized;

            float sinusoidalOffset = Mathf.Sin(elapsedTime * frequency) * amplitude;
            Vector3 sinusoidalTarget = targetPosition + perpendicularDirection * sinusoidalOffset;

            actionList.Add(new AIActionMoveToDestination(sinusoidalTarget));

            return true; // Success
        }
    }
}
