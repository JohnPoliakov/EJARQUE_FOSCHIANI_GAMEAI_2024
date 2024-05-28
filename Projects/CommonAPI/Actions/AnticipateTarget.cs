using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CommonAPI.Actions
{
    public class AnticipateTarget : ActionNode
    {

        private PlayerInformations currentTargetdata;
        private PlayerInformations previousTargetdata;

        public AnticipateTarget(PlayerInformations currentTargetdata, PlayerInformations previousData)
        {
            this.currentTargetdata = currentTargetdata;
            this.previousTargetdata = previousData;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionLookAtPosition(Anticipate()));
            return true; // Success
        }

        public Vector3 Anticipate()
        {

            // Calculer la trajectoire et la vitesse du pion
            Vector3 trajectory = (currentTargetdata.Transform.Position - previousTargetdata.Transform.Position).normalized;
            float speed = Vector3.Distance(previousTargetdata.Transform.Position, currentTargetdata.Transform.Position) / Time.deltaTime;

            // Exemple d'origine du projectile (vous devez la définir en fonction de votre contexte)
            Vector3 projectileOrigin = new Vector3(0, 0, 0);
            float projectileSpeed = 10f; // Exemple de vitesse du projectile

            // Calculer le point d'interception
            Vector3 interceptionPoint = CalculateInterceptionPoint(projectileOrigin, projectileSpeed, currentTargetdata.Transform.Position, trajectory, speed);

            return interceptionPoint;
        }

        private Vector3 CalculateInterceptionPoint(Vector3 origin, float projectileSpeed, Vector3 targetPosition, Vector3 targetTrajectory, float targetSpeed)
        {
            Vector3 toTarget = targetPosition - origin;
            float a = Vector3.Dot(targetTrajectory, targetTrajectory) - Mathf.Pow(projectileSpeed / targetSpeed, 2);
            float b = 2 * Vector3.Dot(toTarget, targetTrajectory);
            float c = Vector3.Dot(toTarget, toTarget);

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // Pas d'interception possible, on tire vers la position actuelle
                return targetPosition;
            }

            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

            float t = Mathf.Max(t1, t2);
            return targetPosition + targetTrajectory * targetSpeed * t;
        }
    }
}
