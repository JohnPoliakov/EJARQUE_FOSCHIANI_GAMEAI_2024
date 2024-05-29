using AI_BehaviorTree_AIGameUtility;
using CommonAPI.Actions;
using CommonAPI.Conditions;
using CommonAPI.TreeBehaviour;
using CommonAPI;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

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


    public class Skynet
    {
        BonusProxy targetedBonusProxy = new BonusProxy();
        TargetProxy targetProxy = new TargetProxy();
        PlayerInformations myPlayerInfos;
        SelectorNode root;

        public Skynet()
        {
            root = new SelectorNode();

            SequenceNode runBonus = new SequenceNode();
            runBonus.AddChild(new IsSeekingBonusNode(() => targetedBonusProxy.Bonus));
            runBonus.AddChild(new MoveToTargetNode(() => targetedBonusProxy.Bonus?.Position ?? Vector3.zero));
            runBonus.AddChild(new IsDashAvailableNode());
            runBonus.AddChild(new InverterNode(new IsTargetInRangeNode(() => targetedBonusProxy.Bonus?.Position ?? Vector3.zero, 1)));
            runBonus.AddChild(new DashToNode(() => targetedBonusProxy.Bonus?.Position ?? Vector3.zero));
            runBonus.AddChild(new HasTargetNode(() => targetProxy.Target));
            runBonus.AddChild(new LookAtTargetNode(() => targetProxy.Target?.Transform.Position ?? Vector3.zero));
            runBonus.AddChild(new FireAtTargetNode());

            root.AddChild(runBonus);

            SequenceNode runTarget = new SequenceNode();
            runTarget.AddChild(new HasTargetNode(() => targetProxy.Target));
            runTarget.AddChild(new LookAtTargetNode(() => targetProxy.Target?.Transform.Position ?? Vector3.zero));
            runTarget.AddChild(new MoveToTargetNode(() => targetProxy.Target?.Transform.Position ?? Vector3.zero));
            runTarget.AddChild(new IsDashAvailableNode());
            runTarget.AddChild(new IsPlayerActive(() => targetProxy.Target?.IsActive ?? false));
            runTarget.AddChild(new DashToNode(() => targetProxy.Target?.Transform.Position ?? Vector3.zero));

            root.AddChild(runTarget);
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

            root.Execute(myPlayerInfos, actionList);

            return actionList;
        }
    }
}
