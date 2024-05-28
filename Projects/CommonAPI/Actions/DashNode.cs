using AI_BehaviorTree_AIGameUtility;
using UnityEngine;
using System.Collections.Generic;

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

            Vector3 direction = ComputeDirection(playerInfo);

            actionList.Add(new AIActionDash(direction));
            return true; // Success
        }


        private Vector3 ComputeDirection(PlayerInformations playerInfo)
        {
            Vector3 positionJoueur = playerInfo.Transform.Position;
            Vector3 directionFuite = Vector3.zero;
            List<ProjectileInformations> projectiles = GetProjectiles(utils, playerInfo);

            if (projectiles.Count == 0)
                return Vector3.forward; // Retourne une direction par défaut si aucun projectile n'est détecté

            // Calculer la direction optimale pour esquiver les projectiles
            float meilleurScore = float.MinValue;
            Vector3 meilleureDirection = Vector3.zero;

            for (int angle = 0; angle < 360; angle += 10) // Parcours des angles à 10° d'intervalle
            {
                Vector3 potentielDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                float score = 0;

                foreach (ProjectileInformations projectile in projectiles)
                {
                    Vector3 toProjectile = projectile.Transform.Position - positionJoueur;
                    float distance = toProjectile.magnitude;
                    float dotProduct = Vector3.Dot(potentielDirection, toProjectile.normalized);

                    // Si le dot product est négatif, le projectile est derrière, ce qui est bon pour nous
                    if (dotProduct < 0)
                    {
                        score += distance / (distance + 1); // +1 pour éviter division par zéro
                    }
                }

                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleureDirection = potentielDirection;
                }
            }

            return meilleureDirection.normalized;
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
