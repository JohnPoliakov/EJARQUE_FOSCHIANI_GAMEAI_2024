using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using CommonAPI.TreeBehaviour;
using CommonAPI.Actions;
using UnityEngine.Assertions;
using CommonAPI.Conditions;
using UnityEngine;

namespace EJARQUE
{
    public class Skynet
    {

        TargetData TargetData { get; set; }
        bool searchBonus;
        BonusInformations targetedBonus;

        public List<AIAction> ComputeAIDecision(int myID, GameWorldUtils utils)
        {
            List<AIAction> actionList = new List<AIAction>();
            PlayerInformations myPlayerInfos = GetPlayerInfos(myID, utils.GetPlayerInfosList());

            PlayerInformations target = SelectTarget(utils, myPlayerInfos);

            if (target == null)
                return actionList;

            SelectorNode root = new SelectorNode();

            SequenceNode attackSequence = new SequenceNode();
            attackSequence.AddChild(new LookAtTargetNode(target.Transform.Position));
            //attackSequence.AddChild(new AnticipateTarget(target, TargetData.informations));
            attackSequence.AddChild(new FireAtTargetNode());
            UpdateTargetData(target);

            SequenceNode moveToTargetSequence = new SequenceNode();
            moveToTargetSequence.AddChild(new LookAtTargetNode(target.Transform.Position));
            if(searchBonus)
                moveToTargetSequence.AddChild(new MoveToTargetNode(targetedBonus.Position));
            else
                moveToTargetSequence.AddChild(new MoveToTargetNode(target.Transform.Position));

            // Dash if is danger
            SequenceNode dashWhenLow = new SequenceNode();
            dashWhenLow.AddChild(new IsDashAvailableNode());
            dashWhenLow.AddChild(new IsInDangerNode(utils, 6));
            dashWhenLow.AddChild(new DashNode(utils, 6));


            // Move to target if not in range
            SequenceNode moveToTargetConditionSequence = new SequenceNode();
            moveToTargetConditionSequence.AddChild(new IsTargetInRangeNode(target, 6.0f));
            moveToTargetConditionSequence.AddChild(moveToTargetSequence);
            //moveToTargetConditionSequence.AddChild(isDashAvailable);
            //moveToTargetConditionSequence.AddChild(new DashNode(target));

           

            // Compile Move and Shoot
            SequenceNode moveAttack = new SequenceNode();
            moveAttack.AddChild(attackSequence);
            moveAttack.AddChild(moveToTargetConditionSequence);
            //moveAttack.AddChild(dashWhenLow);

            root.AddChild(moveAttack);

            root.Execute(target, actionList);

            return actionList;
        }

        private PlayerInformations SelectTarget(GameWorldUtils utils, PlayerInformations myPlayerInfos)
        {
            PlayerInformations targetData = null;
            float targetDistance = float.MaxValue;
            float targetHealth = float.MaxValue;

            searchBonus = false;

            if(utils.GetBonusInfosList().Count > 0)
            {

                foreach (BonusInformations bonusInfo in utils.GetBonusInfosList())
                {
                    if (myPlayerInfos.CurrentHealth <= 0.7f && Vector3.Distance(myPlayerInfos.Transform.Position, bonusInfo.Position) < 5)
                    {
                        searchBonus = true;
                        targetedBonus = bonusInfo;
                        break;
                    }
                }

            }

            


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

                if(targetHealth > playerInfo.CurrentHealth && distanceFromTarget <= 5)
                {
                    targetHealth = playerInfo.CurrentHealth;
                    targetData = playerInfo;
                }

            }

            if(targetData != null && (TargetData == null || TargetData.informations.PlayerId != targetData.PlayerId))
            {
                UpdateTargetData(targetData);
            }

            return targetData;
        }

        private void UpdateTargetData(PlayerInformations informations)
        {
            TargetData = new TargetData(informations);
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
    }


}