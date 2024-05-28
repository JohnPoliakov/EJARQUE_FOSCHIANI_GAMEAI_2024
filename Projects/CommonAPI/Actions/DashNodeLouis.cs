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
            Vector3 directionFuite = Vector3.zero;
            bool isInDanger = false;

            foreach (ProjectileInformations projectile in GetProjectiles(utils, playerInfo))
            {
                Vector3 directionVersProjectile = projectile.Transform.Position - playerInfo.Transform.Position;
                Vector3 directionOpposee = -directionVersProjectile.normalized;

                directionFuite += directionOpposee;
                isInDanger = true;
            }

            // Si le joueur n'est pas en danger, se diriger vers la position cible
            if (!isInDanger)
            {
                return (targetPosition - playerInfo.Transform.Position).normalized;
            }

            return directionFuite.normalized;
        }

        public List<ProjectileInformations> GetProjectiles(GameWorldUtils utils, PlayerInformations playerInfo)
        {
            List<ProjectileInformations> projectiles = new List<ProjectileInformations>();

            foreach (ProjectileInformations projInfo in utils.GetProjectileInfosList())
            {
                if (playerInfo.PlayerId != projInfo.PlayerId && Vector3.Distance(projInfo.Transform.Position, playerInfo.Transform.Position) <= range)
                    projectiles.Add(projInfo);
            }

            return projectiles;
        }
    }
}
