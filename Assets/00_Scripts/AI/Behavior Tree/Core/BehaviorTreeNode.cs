// ==================================================================
// 목적 : Behavior Tree Core 최소 Node 정의
// 생성 일자 : 2025/12/20
// 최근 수정 일자 : 2025/12/20
// ==================================================================

public abstract class BehaviorTreeNode
{
    public string Name { get; }

    protected BehaviorTreeNode(string name = null)
    {
        Name = string.IsNullOrWhiteSpace(name) ? GetType().Name : name;
    }

    public BehaviorTreeState Tick(object context)
    {
        return OnTick(context);
    }

    protected abstract BehaviorTreeState OnTick(object context);
}