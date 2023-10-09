using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SequenceNode : INode
{
    private List<INode> _childs;

    public SequenceNode(List<INode> childs)
    {
        _childs = childs;
    }

    public INode.ENodeState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return INode.ENodeState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Failure:
                    return INode.ENodeState.Failure;
            }
        }

        return INode.ENodeState.Success;
    }
}
