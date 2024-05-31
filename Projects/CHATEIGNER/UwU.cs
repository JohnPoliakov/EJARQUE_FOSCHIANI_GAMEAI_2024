using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using CommonAPI.TreeBehaviour;
using CommonAPI.Actions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;
using CommonAPI.Conditions;
using CommonAPI;

namespace CHATEIGNER
{
    public class UwU
    {
        SelectorNode root;
        PlayerInformations myself;

        PlayerInformations targetPlayer;
        BonusInformations targetBonus;

        public UwU()
        {
            root = new SelectorNode();

            SequenceNode runBonus = new SequenceNode();
            runBonus.AddChild(new IsSeekingBonusNode(() => targetBonus));
            runBonus.AddChild(new MoveToTargetNode(() => targetBonus.Position));
            runBonus.AddChild(new HasTargetNode(() => targetPlayer));
            runBonus.AddChild(new LookAtTargetNode(() => targetPlayer.Transform.Position));
            runBonus.AddChild(new FireAtTargetNode(() => targetPlayer));
            runBonus.AddChild(new IsDashAvailableNode());
            runBonus.AddChild(new DashToNode(() => targetBonus.Position));

            root.AddChild(runBonus);

            SequenceNode runTarget = new SequenceNode();
            runTarget.AddChild(new HasTargetNode(() => targetPlayer));
            runTarget.AddChild(new LookAtTargetNode(() => targetPlayer.Transform.Position));
            runTarget.AddChild(new IsDashAvailableNode());
            runBonus.AddChild(new FireAtTargetNode(() => targetPlayer));
            runBonus.AddChild(new IsSeekingBonusNode(() => targetBonus));
            runTarget.AddChild(new MoveToTargetNode(() => targetPlayer.Transform.Position));
            runBonus.AddChild(new DashToNode(() => targetBonus.Position));

            root.AddChild(runTarget);
        }

        public List<AIAction> ComputeAIDecision(int myID, GameWorldUtils utils)
        {
            List<AIAction> actionList = new List<AIAction>();

            myself = GetPlayerInfos(myID, utils.GetPlayerInfosList());
            targetPlayer = SelectTarget(utils.GetPlayerInfosList());
            targetBonus = BonusToGoToPosition(utils.GetBonusInfosList());

            root.Execute(myself, actionList);

            return actionList;

        }

        public BonusInformations BonusToGoToPosition(List<BonusInformations> bonusList)
        {
            if (bonusList.Count == 0) return null;

            List<BonusInformations> tempHealth = new List<BonusInformations>();
            List<BonusInformations> tempShoot = new List<BonusInformations>();

            foreach (var bonus in bonusList)
            {
                if (bonus.Type == EBonusType.Health)
                    tempHealth.Add(bonus);

                if (bonus.Type == EBonusType.Damage || bonus.Type == EBonusType.CooldownReduction ||
                    bonus.Type == EBonusType.Invulnerability || bonus.Type == EBonusType.BulletSpeed)
                    tempShoot.Add(bonus);
            }

            if(tempHealth.Count > 0)
            {
                if (myself.CurrentHealth < myself.MaxHealth * 0.25)
                {
                    return NearestBonus(tempHealth);
                }
            }

            if (tempShoot.Count > 0)
            {
                return NearestBonus(tempShoot);
            }

            return null;
        }

        public BonusInformations NearestBonus(List<BonusInformations> bonusList)
        {
            var tempDist = float.MaxValue;
            BonusInformations resBonus = bonusList[0];

            foreach (BonusInformations bonus in bonusList)
            {
                var dist = Vector3.Distance(myself.Transform.Position, bonus.Position);
                if (dist < tempDist)
                {
                    resBonus = bonus;
                    tempDist = dist;
                }
            }

            return resBonus;
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

        private PlayerInformations SelectTarget(List<PlayerInformations> playerList)
        {
            PlayerInformations targetData = null;
            float targetDistance = float.MaxValue;

            foreach (PlayerInformations playerInfo in playerList)
            {
                if (!playerInfo.IsActive)
                    continue;

                if (playerInfo.PlayerId == myself.PlayerId)
                    continue;

                float distanceFromTarget = Vector3.Distance(myself.Transform.Position, playerInfo.Transform.Position);

                if (distanceFromTarget < targetDistance)
                {
                    targetData = playerInfo;
                    targetDistance = distanceFromTarget;
                }
            }

            return targetData;
        }
    }
}