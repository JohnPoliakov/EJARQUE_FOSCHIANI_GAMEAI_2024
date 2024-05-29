using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using CommonAPI.TreeBehaviour;
using CommonAPI.Actions;
using UnityEngine.Assertions;
using CommonAPI.Conditions;
using CommonAPI;
using System;
using UnityEngine;

namespace FOSCHIANI
{
    public class RATIO
    {
        private Dictionary<int, Vector3> previousPositions = new Dictionary<int, Vector3>();

        public List<AIAction> ComputeAIDecision(int myID, GameWorldUtils utils)
        {
            List<AIAction> actionList = new List<AIAction>();
            PlayerInformations myPlayerInfos = GetPlayerInfos(myID, utils.GetPlayerInfosList());

            PlayerInformations target = SelectTarget(utils.GetPlayerInfosList(), myPlayerInfos);
            BonusInformations targetBonus = SelectBonus(utils.GetBonusInfosList(), myPlayerInfos);

            if (target == null && targetBonus == null)
                return actionList;

            if (target != null && !previousPositions.ContainsKey(target.PlayerId))
            {
                previousPositions[target.PlayerId] = target.Transform.Position;
            }

            Vector3 predictedPosition = target != null ? PredictTargetPosition(target) : Vector3.zero;

            SelectorNode root = new SelectorNode();

            // Always look at the predicted position if a target exists
            SequenceNode lookAtSequence = new SequenceNode();
            lookAtSequence.AddChild(new LookAtTargetNode(predictedPosition));

            // If there's a target, attack it
            if (target != null)
            {
                SequenceNode attackSequence = new SequenceNode();
                attackSequence.AddChild(lookAtSequence);
                attackSequence.AddChild(new FireAtTargetNode());
                root.AddChild(attackSequence);
            }

            // If there's a bonus, move towards it
            if (targetBonus != null)
            {
                SequenceNode moveToBonusSequence = new SequenceNode();
                moveToBonusSequence.AddChild(new LookAtTargetNode(targetBonus.Position));
                moveToBonusSequence.AddChild(new MoveToTargetNode(targetBonus.Position));

                // Dash to reach the bonus faster
                SequenceNode dashToBonusSequence = new SequenceNode();
                dashToBonusSequence.AddChild(new IsDashAvailableNode());
                dashToBonusSequence.AddChild(new DashNodeLouis(utils, targetBonus.Position, 15));

                root.AddChild(dashToBonusSequence);
                root.AddChild(moveToBonusSequence);
            }
            else if (target != null) // If no bonus, move towards the target
            {
                SequenceNode moveToTargetSequence = new SequenceNode();
                moveToTargetSequence.AddChild(lookAtSequence);
                moveToTargetSequence.AddChild(new MoveToTargetNode(predictedPosition));

                // Dash if possible
                SequenceNode dashToTargetSequence = new SequenceNode();
                dashToTargetSequence.AddChild(new IsDashAvailableNode());
                dashToTargetSequence.AddChild(new DashNodeLouis(utils, predictedPosition, 15));

                root.AddChild(dashToTargetSequence);
                root.AddChild(moveToTargetSequence);
            }

            // Execute the root node with the correct context
            if (target != null) root.Execute(target, actionList);
            else root.Execute(targetBonus, actionList);

            if (target != null)
            {
                previousPositions[target.PlayerId] = target.Transform.Position;
            }

            return actionList;
        }

        private Vector3 PredictTargetPosition(PlayerInformations target)
        {
            if (!previousPositions.ContainsKey(target.PlayerId))
            {
                previousPositions[target.PlayerId] = target.Transform.Position;
                return target.Transform.Position;
            }

            Vector3 previousPosition = previousPositions[target.PlayerId];

            // Calculate the velocity using the current and previous positions
            Vector3 velocity = (target.Transform.Position - previousPosition) / Time.deltaTime;

            // Predict the future position using the calculated velocity
            return target.Transform.Position + velocity * Time.deltaTime;
        }

        private PlayerInformations SelectTarget(List<PlayerInformations> playerInfos, PlayerInformations myPlayerInfos)
        {
            PlayerInformations closestTarget = null;
            PlayerInformations lowestHealthTarget = null;
            float closestDistance = float.MaxValue;
            float lowestHealth = float.MaxValue;

            foreach (PlayerInformations playerInfo in playerInfos)
            {
                if (!playerInfo.IsActive || playerInfo.PlayerId == myPlayerInfos.PlayerId)
                    continue;

                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, playerInfo.Transform.Position);

                // Check for closest target within a radius of 5 units
                if (distance < 5.0f)
                {
                    closestTarget = playerInfo;
                    break;
                }

                // Find the player with the lowest health
                if (playerInfo.CurrentHealth < lowestHealth)
                {
                    lowestHealthTarget = playerInfo;
                    lowestHealth = playerInfo.CurrentHealth;
                }
            }

            return closestTarget ?? lowestHealthTarget;
        }

        private BonusInformations SelectBonus(List<BonusInformations> bonusInfos, PlayerInformations myPlayerInfos)
        {
            BonusInformations closestBonus = null;
            float closestDistance = float.MaxValue;

            foreach (BonusInformations bonusInfo in bonusInfos)
            {
                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, bonusInfo.Position);

                if (distance < closestDistance)
                {
                    closestBonus = bonusInfo;
                    closestDistance = distance;
                }
            }

            return closestBonus;
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
