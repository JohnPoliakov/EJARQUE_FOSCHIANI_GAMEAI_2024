using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Actions
{
    public class MoveDodgeToTargetNode : ActionNode
    {
        private Vector3 targetPosition;

        public MoveDodgeToTargetNode(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionMoveToDestination(GetLateralPosition(playerInfo.Transform.Position, (targetPosition - playerInfo.Transform.Position).normalized)));
            return true; // Success
        }

        Vector3 GetLateralPosition(Vector3 currentPosition, Vector3 direction)
        {
            // Calculer les directions latérales gauche et droite
            Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * direction;

            // Vérifier la direction gauche
            if (IsDirectionClear(currentPosition, leftDirection))
            {
                return currentPosition + leftDirection * 3;
            }

            // Vérifier la direction droite
            if (IsDirectionClear(currentPosition, rightDirection))
            {
                return currentPosition + rightDirection * 3;
            }

            // Si aucune direction n'est dégagée, retourner Vector3.zero
            return Vector3.zero;
        }

        bool IsDirectionClear(Vector3 position, Vector3 direction)
        {
            // Utiliser un Raycast pour vérifier la présence d'obstacles
            Ray ray = new Ray(position, direction);
            return !Physics.Raycast(ray, 3);
        }
    }
}
