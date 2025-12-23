using UnityEngine;

// ==================================================================
// 목적 : Behavior Tree Core 디버거용 실행기
// 생성 일자 : 2025/12/20
// 최근 수정 일자 : 2025/12/20
// ==================================================================

// 간단한 Behavior Tree 예제 실행기
// 0,1,2 중 랜덤 값을 선택하여 해당 값에 맞는 분기 실행

public class BTDebugRunner : MonoBehaviour
{
    private int _value;

    private BehaviorTreeNode _root;

    private void Awake()
    {
        _value = Random.Range(0, 3); // 0,1,2

        _root = new SelectorNode(new BehaviorTreeNode[]
        {
            new SequenceNode(new BehaviorTreeNode[]
            {
                new ConditionNode(ctx => ((int)ctx) == 0, "IsZero"),
                new ActionNode(ctx =>
                {
                    Debug.Log("[Behavior Tree] Pick: Zero Branch");
                    return BehaviorTreeState.Success;
                }, "DoZero")
            }, "SeqZero"),

            new SequenceNode(new BehaviorTreeNode[]
            {
                new ConditionNode(ctx => ((int)ctx) == 1, "IsOne"),
                new ActionNode(ctx =>
                {
                    Debug.Log("[Behavior Tree] Pick: One Branch");
                    return BehaviorTreeState.Success;
                }, "DoOne")
            }, "SeqOne"),
        }, "RootSelector");
    }

    private void Start()
    {
        var result = _root.Tick(_value);
        Debug.Log($"[Behavior Tree] ContextValue={_value}, Result={result}");
    }
}
