using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace CommonAPI.Conditions
{
    public class IsInDangerNode : ConditionNode
    {

        private GameWorldUtils utils;
        private float range;

        public IsInDangerNode(GameWorldUtils utils, float range)
        {
            this.utils = utils;
            this.range = range;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {

            float health = playerInfo.CurrentHealth / playerInfo.MaxHealth;
            bool isLow = health <= 0.5f || GetProjectiles(utils, playerInfo).Count >= 3;

            

            return isLow;
        }

        public List<ProjectileInformations> GetProjectiles(GameWorldUtils utils, PlayerInformations playerInfo) {

            List<ProjectileInformations> projectiles = new List<ProjectileInformations>();

            foreach(ProjectileInformations projInfo in utils.GetProjectileInfosList())
            {
                if(playerInfo.PlayerId != projInfo.PlayerId && Vector3.Distance(projInfo.Transform.Position, playerInfo.Transform.Position) <= range)
                    projectiles.Add(projInfo);
            }

            return projectiles;
        }

    }
}
