using AI_BehaviorTree_AIGameUtility;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class DashNode : ActionNode
    {

        private Func<GameWorldUtils> utils;

        public DashNode(Func<GameWorldUtils> utils)
        {
            this.utils = utils;
        }

        public override bool Execute(PlayerInformations playerInfo, List<AIAction> actionList)
        {

            Vector3 direction = ComputeDirection(playerInfo);

            if(direction != Vector3.zero)
                actionList.Add(new AIActionDash(direction));
            else
            {
                actionList.Add(new AIActionDash(GetRandomDirection(playerInfo)));
            }
            return true; // Success
        }


        private Vector3 GetRandomDirection(PlayerInformations playerInfo)
        {
            float num = UnityEngine.Random.Range(-1f, 1f);
            float num2 = UnityEngine.Random.Range(-1f, 1f);
            Vector3 normalized = new Vector3(num, 0f, num2).normalized;
            return playerInfo.Transform.Position + normalized * 5f;
        }

        private Vector3 ComputeDirection(PlayerInformations playerInfo)
        {
            Vector3 positionJoueur = playerInfo.Transform.Position;
            ProjectileInformations projectile = GetProjectile(utils.Invoke(), playerInfo);

            if (projectile == null)
                return Vector3.zero; // Retourne une direction par défaut si aucun projectile n'est détecté

            Vector3 directionFromProjToPlayer = projectile.Transform.Position - positionJoueur;

            return Vector3.Cross(directionFromProjToPlayer.normalized, Vector3.up).normalized;
        }


        public ProjectileInformations GetProjectile(GameWorldUtils utils, PlayerInformations playerInfo)
        {

            ProjectileInformations projectile = null;
            float distance = Mathf.Infinity;

            foreach (ProjectileInformations projInfo in utils.GetProjectileInfosList())
            {
                float dist = Vector3.Distance(projInfo.Transform.Position, playerInfo.Transform.Position);

                if (playerInfo.PlayerId != projInfo.PlayerId && dist < distance)
                {
                    distance = dist;
                    projectile = projInfo;
                }
            }

            return projectile;
        }
    }
}
