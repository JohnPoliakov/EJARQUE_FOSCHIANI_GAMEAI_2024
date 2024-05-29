using AI_BehaviorTree_AIGameUtility;
using System;

namespace CommonAPI.Actions
{
    public class IsPlayerImmortalNode : ConditionNode
    {

        private Func<bool> _isPlayerActive;

        public IsPlayerImmortalNode(Func<bool> isPlayerActive)
        {
            _isPlayerActive = isPlayerActive;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return _isPlayerActive.Invoke();
        }

    }
}
