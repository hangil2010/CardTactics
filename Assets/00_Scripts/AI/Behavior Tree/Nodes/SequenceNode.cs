using System;
using System.Collections.Generic;

// ==================================================================
// 목적 : Sequence 노드 - 자식 모두 Success면 Success, 하나라도 Failure면 Failure
// 생성 일자 : 2025/12/20
// 최근 수정 일자 : 2025/12/20
// ==================================================================

public sealed class SequenceNode : BehaviorTreeNode
{
    private readonly List<BehaviorTreeNode> _children;

    public SequenceNode(IEnumerable<BehaviorTreeNode> children, string name = null) : base(name)
    {
        if (children == null) throw new ArgumentNullException(nameof(children));
        _children = new List<BehaviorTreeNode>(children);
    }

    protected override BehaviorTreeState OnTick(object context)
    {
        for (int i = 0; i < _children.Count; i++)
        {
            var state = _children[i].Tick(context);
            if (state == BehaviorTreeState.Failure)
                return BehaviorTreeState.Failure;
        }
        return BehaviorTreeState.Success;
    }
}

