using AI_BehaviorTree_AIGameUtility;
using System;

namespace CommonAPI.Actions
{
    public class IsPlayerActive : ConditionNode
    {

        private Func<bool> _isPlayerActive;

        public IsPlayerActive(Func<bool> isPlayerActive)
        {
            _isPlayerActive = isPlayerActive;
        }

        public override bool Evaluate(PlayerInformations playerInfo)
        {
            return _isPlayerActive.Invoke();
        }

    }
}
