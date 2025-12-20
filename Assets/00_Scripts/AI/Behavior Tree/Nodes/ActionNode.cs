using System;

// ==================================================================
// 목적 : 액션 노드 - 성공/실패만 반환하는 최소 액션 노드
// 생성 일자 : 2025/12/20
// 최근 수정 일자 : 2025/12/20
// ==================================================================

public sealed class ActionNode : BehaviorTreeNode
{
    private readonly Func<object, BehaviorTreeState> _action;

    public ActionNode(Func<object, BehaviorTreeState> action, string name = null) : base(name)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    protected override BehaviorTreeState OnTick(object context)
    {
        return _action(context);
    }
}
