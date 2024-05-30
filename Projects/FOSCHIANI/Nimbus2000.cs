using AI_BehaviorTree_AIGameUtility;
using CommonAPI.Actions;
using CommonAPI.Conditions;
using CommonAPI.TreeBehaviour;
using CommonAPI;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace FOSCHIANI
{
    public class TargetProxy
    {
        public PlayerInformations Target { get; set; }
    }

    public class BonusProxy
    {
        public BonusInformations Bonus { get; set; }
    }

    public class Nimbus2000
    {
        BonusProxy targetedBonusProxy = new BonusProxy();
        TargetProxy targetProxy = new TargetProxy();
        PlayerInformations myPlayerInfos;
        SelectorNode root;

        // Positions précédentes et actuelle pour calculer la vitesse de la cible
        Vector3 previousTargetPosition;
        Vector3 currentTargetPosition;

        public Nimbus2000()
        {
            root = new SelectorNode();

            // Séquence pour obtenir un bonus
            SequenceNode runBonus = new SequenceNode();
            runBonus.AddChild(new IsSeekingBonusNode(() => targetedBonusProxy.Bonus));
            runBonus.AddChild(new MoveToTargetNode(() => targetedBonusProxy.Bonus?.Position ?? GetRandomDirection()));
            runBonus.AddChild(new HasTargetNode(() => targetProxy.Target));
            runBonus.AddChild(new LookAtTargetNode(() => AnticipateTargetPosition()));
            runBonus.AddChild(new FireAtTargetNode(() => targetProxy.Target));
            runBonus.AddChild(new IsDashAvailableNode());
            runBonus.AddChild(new InverterNode(new IsTargetInRangeNode(() => targetedBonusProxy.Bonus?.Position ?? GetRandomDirection(), 1)));
            runBonus.AddChild(new DashToNode(() => targetedBonusProxy.Bonus?.Position ?? GetRandomDirection()));

            root.AddChild(runBonus);

            // Séquence si pas de bonus
            SequenceNode runTarget = new SequenceNode();
            runTarget.AddChild(new HasTargetNode(() => targetProxy.Target));
            runTarget.AddChild(new LookAtTargetNode(() => AnticipateTargetPosition()));
            runTarget.AddChild(new LookAtTargetNode(() => AnticipateTargetPosition()));
            runTarget.AddChild(new IsDashAvailableNode());
            runTarget.AddChild(new InverterNode(new IsTargetInRangeNode(() => targetProxy.Target?.Transform.Position ?? GetRandomDirection(), 5)));
            runTarget.AddChild(new MoveToTargetNode(() => targetProxy.Target?.Transform.Position ?? GetRandomDirection()));
            runTarget.AddChild(new DashToNode(() => targetProxy.Target?.Transform.Position ?? GetRandomDirection()));

            root.AddChild(runTarget);
        }

        private BonusInformations SelectBonus(GameWorldUtils utils, PlayerInformations myPlayerInfos)
        {
            BonusInformations closestBonus = null;
            float minDistance = float.MaxValue;

            foreach (BonusInformations bonusInfo in utils.GetBonusInfosList())
            {
                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, bonusInfo.Position);

                if (distance < minDistance)
                {
                    closestBonus = bonusInfo;
                    minDistance = distance;
                }
            }

            return closestBonus;
        }

        private PlayerInformations SelectTarget(GameWorldUtils utils, PlayerInformations myPlayerInfos)
        {
            PlayerInformations targetData = null;
            float targetDistance = float.MaxValue;
            float targetHealth = float.MaxValue;

            foreach (PlayerInformations playerInfo in utils.GetPlayerInfosList())
            {
                if (!playerInfo.IsActive)
                    continue;

                if (playerInfo.PlayerId == myPlayerInfos.PlayerId)
                    continue;

                if (utils.GetPlayerInfosList().Count > 2 && playerInfo.BonusOnPlayer[EBonusType.Invulnerability] >= 0)
                    continue;

                float distanceFromTarget = Vector3.Distance(myPlayerInfos.Transform.Position, playerInfo.Transform.Position);

                if (distanceFromTarget < targetDistance)
                {
                    targetData = playerInfo;
                    targetDistance = distanceFromTarget;
                }

                if (targetHealth > playerInfo.CurrentHealth && distanceFromTarget <= 5)
                {
                    targetHealth = playerInfo.CurrentHealth;
                    targetData = playerInfo;
                }
            }

            return targetData;
        }

        public PlayerInformations GetPlayerInfos(int parPlayerId, List<PlayerInformations> parPlayerInfosList)
        {
            foreach (PlayerInformations playerInfo in parPlayerInfosList)
            {
                if (playerInfo.PlayerId == parPlayerId)
                    return playerInfo;
            }

            Assert.IsTrue(false, "GetPlayerInfos : PlayerId not Found");
            return null;
        }

        public List<AIAction> ComputeAIDecision(int myID, GameWorldUtils utils)
        {
            List<AIAction> actionList = new List<AIAction>();
            myPlayerInfos = GetPlayerInfos(myID, utils.GetPlayerInfosList());

            targetProxy.Target = SelectTarget(utils, myPlayerInfos);
            targetedBonusProxy.Bonus = SelectBonus(utils, myPlayerInfos);

            if (targetProxy.Target != null && targetProxy.Target.IsActive)
            {
                // Mettre à jour les positions précédente et actuelle de la cible
                if (previousTargetPosition == Vector3.zero)
                {
                    previousTargetPosition = targetProxy.Target.Transform.Position;
                }
                else
                {
                    previousTargetPosition = currentTargetPosition;
                }

                currentTargetPosition = targetProxy.Target.Transform.Position;
            }

            root.Execute(myPlayerInfos, actionList);

            return actionList;
        }

        private Vector3 AnticipateTargetPosition()
        {
            if (targetProxy.Target == null || previousTargetPosition == Vector3.zero || currentTargetPosition == Vector3.zero)
            {
                return Vector3.zero;
            }

            // Calcul de la vitesse et de la direction de la cible
            Vector3 targetVelocity = (currentTargetPosition - previousTargetPosition) / Time.deltaTime;

            // Vérification de la validité de la vitesse
            if (targetVelocity == Vector3.zero)
            {
                return currentTargetPosition;
            }

            float estimatedTimeOfFlight = 0.05f;

            Vector3 predictedPosition = currentTargetPosition + targetVelocity * estimatedTimeOfFlight;

            if (Mathf.Abs(predictedPosition.y - currentTargetPosition.y) > 1.0f)
            {
                predictedPosition.y = currentTargetPosition.y;
            }

            // Limiter les mouvements horizontaux excessifs
            float maxVerticalDistance = 1.0f; // Ajustez cette valeur selon votre jeu
            if (Vector3.Distance(currentTargetPosition, predictedPosition) > maxVerticalDistance)
            {
                predictedPosition = currentTargetPosition + targetVelocity.normalized * maxVerticalDistance;
            }

            return predictedPosition;
        }

        private Vector3 GetRandomDirection()
        {
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            Vector3 randomDirection = new Vector3(randomX, 0, randomZ).normalized;
            return myPlayerInfos.Transform.Position + randomDirection * 5f; // Se déplacer dans une direction aléatoire à 5 unités de distance
        }
    }
}
