using System.Collections.Generic;

namespace CommonAPI
{
    public abstract class ControlNode : ActionNode
    {
        protected List<ActionNode> children = new List<ActionNode>();

        public void AddChild(ActionNode child)
        {
            children.Add(child);
        }


    }
}
