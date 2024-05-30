using AI_BehaviorTree_AIGameUtility;
using CommonAPI.Actions;
using CommonAPI.Conditions;
using CommonAPI.TreeBehaviour;
using CommonAPI;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.UIElements;

namespace EJARQUE
{
    public class TargetProxy
    {
        public PlayerInformations Target { get; set; }
    }

    public class BonusProxy
    {
        public BonusInformations Bonus { get; set; }
    }

    public class WorldProxy
    {
        public GameWorldUtils PreviousWorld { get; set; }
        public GameWorldUtils CurrentWorld { get; set; }
    }


    public class Skynet
    {
        BonusProxy targetedBonusProxy = new BonusProxy();
        TargetProxy targetProxy = new TargetProxy();
        WorldProxy worldProxy = new WorldProxy();
        PlayerInformations myPlayerInfos;
        SelectorNode root;

        public Skynet()
        {
            root = new SelectorNode();


            SequenceNode dash = new SequenceNode();
            dash.AddChild(new IsDashAvailableNode());
            dash.AddChild(new DashNode(()=> worldProxy.CurrentWorld));

            SequenceNode runBonusAndShoot = new SequenceNode();
            runBonusAndShoot.AddChild(new HasTargetNode(() => targetProxy.Target));
            runBonusAndShoot.AddChild(new LookAtTargetNode(() => AnticipateTargetPosition()));
            runBonusAndShoot.AddChild(new FireAtTargetNode(() => targetProxy.Target));
            runBonusAndShoot.AddChild(new IsSeekingBonusNode(() => targetedBonusProxy.Bonus));
            runBonusAndShoot.AddChild(new MoveToTargetNode(() => targetedBonusProxy.Bonus?.Position ?? myPlayerInfos.Transform.Position));

            SequenceNode runTargetAndShoot = new SequenceNode();
            runTargetAndShoot.AddChild(new HasTargetNode(() => targetProxy.Target));
            runTargetAndShoot.AddChild(new LookAtTargetNode(() => AnticipateTargetPosition()));
            runTargetAndShoot.AddChild(new FireAtTargetNode(() => targetProxy.Target));
            runTargetAndShoot.AddChild(new InverterNode(new IsTargetInRangeNode(() => targetProxy.Target.Transform.Position, 4)));
            runTargetAndShoot.AddChild(new MoveToTargetNode(() => targetProxy.Target?.Transform.Position ?? myPlayerInfos.Transform.Position));

            SequenceNode seekBonus = new SequenceNode();
            seekBonus.AddChild(new IsSeekingBonusNode(() => targetedBonusProxy.Bonus));
            seekBonus.AddChild(new MoveToTargetNode(() => targetedBonusProxy.Bonus?.Position ?? myPlayerInfos.Transform.Position));
            
            SequenceNode dashRandom = new SequenceNode();
            dashRandom.AddChild(new IsDashAvailableNode());
            dashRandom.AddChild(new DashToNode(() => GetRandomDirection(myPlayerInfos)));
            
            SequenceNode moveRandom = new SequenceNode();
            moveRandom.AddChild(new MoveToTargetNode(() => GetRandomDirection(myPlayerInfos)));

            SequenceNode stop = new SequenceNode();
            stop.AddChild(new StopMovementNode());

            root.AddChild(dash);
            root.AddChild(runBonusAndShoot);
            root.AddChild(runTargetAndShoot);
            root.AddChild(seekBonus);
            root.AddChild(dashRandom);
            root.AddChild(moveRandom);
            root.AddChild(stop);

        }

        private Vector3 GetRandomDirection(PlayerInformations playerInfo)
        {
            float num = Random.Range(-1f, 1f);
            float num2 = Random.Range(-1f, 1f);
            Vector3 normalized = new Vector3(num, 0f, num2).normalized;
            return playerInfo.Transform.Position + normalized * 5f;
        }

        private BonusInformations SelectBonus(GameWorldUtils utils, PlayerInformations myPlayerInfos)
        {
            BonusInformations bonus = null;
            float targetDistance = float.MaxValue;

            foreach (BonusInformations bonusInfo in utils.GetBonusInfosList())
            {
                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, bonusInfo.Position);

                if (distance < targetDistance)
                {
                    bonus = bonusInfo;
                    targetDistance = distance;
                }
            }

            return bonus;
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

            worldProxy.PreviousWorld = worldProxy.CurrentWorld;
            worldProxy.CurrentWorld = utils;

            root.Execute(myPlayerInfos, actionList);

            return actionList;
        }

        // Méthode pour calculer la position anticipée de la cible
        private Vector3 AnticipateTargetPosition()
        {
            if (targetProxy.Target == null || worldProxy.PreviousWorld == null) return myPlayerInfos.Transform.Position;

            PlayerInformations previousTargerData = null;

            foreach(PlayerInformations previousPlayerData in worldProxy.PreviousWorld.GetPlayerInfosList()){
                if(previousPlayerData.PlayerId == targetProxy.Target.PlayerId)
                {
                    previousTargerData = previousPlayerData;
                    break;
                }
            }

            if (previousTargerData == null) return targetProxy.Target.Transform.Position;

            Vector3 currentPosition = targetProxy.Target.Transform.Position;
            Vector3 previousPosition = previousTargerData.Transform.Position;
            Vector3 direction = (currentPosition - previousPosition).normalized;
            Vector3 targetVelocity = (currentPosition - previousPosition) / Time.deltaTime;

            float estimatedTimeOfFlight = 0.05f;

            Vector3 predictedPosition = currentPosition + targetVelocity * estimatedTimeOfFlight;

            if (Mathf.Abs(predictedPosition.y - currentPosition.y) > 1.0f)
            {
                predictedPosition.y = currentPosition.y;
            }

            // Limiter les mouvements horizontaux excessifs
            float maxVerticalDistance = 1.0f; // Ajustez cette valeur selon votre jeu
            if (Vector3.Distance(currentPosition, predictedPosition) > maxVerticalDistance)
            {
                predictedPosition = currentPosition + targetVelocity.normalized * maxVerticalDistance;
            }

            return predictedPosition;

        }
    }
}
