using System;

// ==================================================================
// 목적 : 조건 노드 - 조건이 참이면 Success, 거짓이면 Failure
// 생성 일자 : 2025/12/20
// 최근 수정 일자 : 2025/12/20
// ==================================================================

public sealed class ConditionNode : BehaviorTreeNode
{
    private readonly Func<object, bool> _predicate;

    public ConditionNode(Func<object, bool> predicate, string name = null) : base(name)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }

    protected override BehaviorTreeState OnTick(object context)
    {
        return _predicate(context) ? BehaviorTreeState.Success : BehaviorTreeState.Failure;
    }
}

