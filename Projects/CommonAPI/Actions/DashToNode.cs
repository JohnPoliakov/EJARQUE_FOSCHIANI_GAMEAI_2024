using AI_BehaviorTree_AIGameUtility;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CommonAPI.Actions
{
    public class DashToNode : ActionNode
    {

        private Func<Vector3> to;

        public DashToNode(Func<Vector3> to)
        {
            this.to = to;
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
            
            return (to.Invoke()-positionJoueur).normalized;
        }

    
    }
}
