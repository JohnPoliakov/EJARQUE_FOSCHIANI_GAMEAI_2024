using AI_BehaviorTree_AIGameUtility;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.Diagnostics;
using Windows.Services.Maps;

namespace CommonAPI.Actions
{
    public class DashNode : ActionNode
    {

        private GameWorldUtils utils;
        private float range;

        public DashNode(GameWorldUtils utils, float range)
        {
            this.utils = utils;
            this.range = range;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {
            actionList.Add(new AIActionDash(ComputeDirection(playerInfo)));
            return true; // Success
        }


        private Vector3 ComputeDirection(PlayerInformations playerInfo)
        {
            Vector3 directionFuite = Vector3.zero;

            foreach (ProjectileInformations projectile in GetProjectiles(utils, playerInfo))
            {
                Vector3 directionVersProjectile = projectile.Transform.Position - playerInfo.Transform.Position;
                Vector3 directionOpposee = -directionVersProjectile.normalized;

                directionFuite += directionOpposee;
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
