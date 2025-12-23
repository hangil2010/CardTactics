using UnityEngine;

// BehaviorTreeNode, SelectorNode, SequenceNode, ConditionNode, ActionNode,
// BehaviorTreeState 는 이미 Core에 존재한다고 가정

public static class BehaviorTreeBuilder
{
    public static BehaviorTreeNode Build()
    {
        return new SelectorNode(new BehaviorTreeNode[]
        {
            // 1) 첫 의사 결정 시 랜덤 패턴 선택
            new SequenceNode(new BehaviorTreeNode[]
            {
                new ConditionNode(ctx => IsFirstDecision(ctx)),
                new ActionNode(ctx =>
                {
                    ApplyRandomPattern((TurnContext)ctx);
                    return BehaviorTreeState.Success;
                })
            }),

            // 2) 이후 체력 상태에 따른 패턴 선택
            new SelectorNode(new BehaviorTreeNode[]
            {
                // 체력 낮음, 공격 패턴 실행
                new SequenceNode(new BehaviorTreeNode[]
                {
                    new ConditionNode(ctx => IsEnemyHpLower(ctx)),
                    new ActionNode(ctx =>
                    {
                        ApplyAttackPattern((TurnContext)ctx);
                        return BehaviorTreeState.Success;
                    })
                }),
                // 체력 같음, 방어 패턴 실행
                new SequenceNode(new BehaviorTreeNode[]
                {
                    new ConditionNode(ctx => IsEnemyHpEqual(ctx)),
                    new ActionNode(ctx =>
                    {
                        ApplyDefensePattern((TurnContext)ctx);
                        return BehaviorTreeState.Success;
                    })
                }),
                // 체력 높음, 회복 패턴 실행
                new SequenceNode(new BehaviorTreeNode[]
                {
                    new ConditionNode(ctx => IsEnemyHpHigher(ctx)),
                    new ActionNode(ctx =>
                    {
                        ApplyHealPattern((TurnContext)ctx);
                        return BehaviorTreeState.Success;
                    })
                }),
            }),

            // 3) 기본 방어 패턴 실행, Fallback
            new ActionNode(ctx =>
            {
                ApplyDefensePattern((TurnContext)ctx);
                return BehaviorTreeState.Success;
            })
        });
    }
    // 조건 노드 함수들
    // TurnContext 의 aiDecisionCount 가 0 인지 확인
    // 첫 의사 결정(첫 턴)인지 확인
    private static bool IsFirstDecision(object context)
    {
        return ((TurnContext)context).aiDecisionCount == 0;
    }
    // 적과 플레이어의 체력 비교 함수들
    // 적 체력이 더 낮은지 확인
    private static bool IsEnemyHpLower(object context)
    {
        var ctx = (TurnContext)context;
        return ctx.enemyCharactor.GetHealth() < ctx.playerCharactor.GetHealth();
    }
    // 적 체력이 플레이어와 같은지 확인
    private static bool IsEnemyHpEqual(object context)
    {
        var ctx = (TurnContext)context;
        return ctx.enemyCharactor.GetHealth() == ctx.playerCharactor.GetHealth();
    }
    // 적 체력이 더 높은지 확인
    private static bool IsEnemyHpHigher(object context)
    {
        var ctx = (TurnContext)context;
        return ctx.enemyCharactor.GetHealth() > ctx.playerCharactor.GetHealth();
    }

    // 가중치 (3,1,1) → 60% / 20% / 20%
    // 각 패턴마다 해당하는 이름의 가중치가 높게 나오도록 설정
    private static void ApplyAttackPattern(TurnContext ctx)
    {
        ctx.aiAttackWeight = 3;
        ctx.aiDefenseWeight = 1;
        ctx.aiHealWeight = 1;
    }

    private static void ApplyDefensePattern(TurnContext ctx)
    {
        ctx.aiAttackWeight = 1;
        ctx.aiDefenseWeight = 3;
        ctx.aiHealWeight = 1;
    }

    private static void ApplyHealPattern(TurnContext ctx)
    {
        ctx.aiAttackWeight = 1;
        ctx.aiDefenseWeight = 1;
        ctx.aiHealWeight = 3;
    }

    // 완전 랜덤 선택
    private static void ApplyRandomPattern(TurnContext ctx)
    {
        int r = Random.Range(0, 3);
        switch (r)
        {
            case 0: ApplyAttackPattern(ctx); break;
            case 1: ApplyDefensePattern(ctx); break;
            default: ApplyHealPattern(ctx); break;
        }
    }
}
