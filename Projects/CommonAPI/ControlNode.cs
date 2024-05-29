using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace CommonAPI
{
    public abstract class ControlNode : ActionNode
    {

        BonusInformations bonus;
        PlayerInformations target;

        protected List<ActionNode> children = new List<ActionNode>();

        public void AddChild(ActionNode child)
        {
            children.Add(child);
        }


    }
}
