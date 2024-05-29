using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using System;

namespace CommonAPI.Conditions
{
    public class IsInDangerNode : ConditionNode
    {

        private Func<GameWorldUtils> utils;
        private float range;

        public IsInDangerNode(Func<GameWorldUtils> utils, float range)
        {
            this.utils = utils;
            this.range = range;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {

            float health = playerInfo.CurrentHealth / playerInfo.MaxHealth;
            bool isLow = health <= 0.5f || GetProjectiles(utils.Invoke(), playerInfo).Count > 0;

            return isLow;
        }

        public List<ProjectileInformations> GetProjectiles(GameWorldUtils utils, PlayerInformations playerInfo) {

            List<ProjectileInformations> projectiles = new List<ProjectileInformations>();

            foreach(ProjectileInformations projInfo in utils.GetProjectileInfosList())
            {
                if(playerInfo.PlayerId != projInfo.PlayerId)
                    projectiles.Add(projInfo);
            }

            return projectiles;
        }

    }
}
