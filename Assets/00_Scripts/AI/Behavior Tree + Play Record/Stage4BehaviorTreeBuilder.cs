using UnityEngine;

// ==================================================================
// 목적 : Stage 4 AI 판단 로직(플레이 기록 기반)을 Behavior Tree 구조로 정의한다
// 생성 일자 : 25/12/22
// 최근 수정 일자 : 25/12/22
// ==================================================================

public static class Stage4BehaviorTreeBuilder
{
    // 튜닝 파라미터
    private const int MinSamplesPerSlot = 5;
    // 슬롯별 우세 판단 임계값
    public const float DominantThreshold = 0.6f;

    // 우세한 플레이 경향성 유형
    private enum Dominant
    {
        None = 0,
        Attack = 1,
        Defense = 2,
        Heal = 3
    }
    /// <summary>
    /// Stage 4 : Behavior Tree + Play Record를 합성한 빌드
    /// </summary>
    /// <returns></returns>
    public static BehaviorTreeNode Build()
    {
        // 기록의 정확성을 위해 Selector 노드 사용
        // 기록이 충분하면 Stage4(카운터) 적용, 부족하면 Stage3(HP 비교)로 fallback
        return new SelectorNode(new BehaviorTreeNode[]
        {
            // 1) 기록이 충분하면 Stage4(카운터) 가중치 적용
            new SequenceNode(new BehaviorTreeNode[]
            {
                new ConditionNode(ctx => HasEnoughRecord((TurnContext)ctx)),
                new ActionNode(ctx =>
                {
                    ApplyCounterWeightsForAllSlots((TurnContext)ctx);
                    return BehaviorTreeState.Success;
                })
            }),

            // 2) 기록이 부족하면 Stage3(HP 비교)로 fallback
            new ActionNode(ctx =>
            {
                ApplyStage3WeightsForAllSlots((TurnContext)ctx);
                return BehaviorTreeState.Success;
            })
        });
    }

    /// <summary>
    /// 충분한 기록이 있는지 확인
    /// </summary>
    private static bool HasEnoughRecord(TurnContext ctx)
    {
        if (ctx == null) return false;
        if (ctx.playRecord == null) return false;

        // Player 기록을 기준으로 “상대 경향성”을 판단 (AI 입장)
        return ctx.playRecord.HasEnoughSamples(PlayRecord.Actor.Player, 0, MinSamplesPerSlot)
            && ctx.playRecord.HasEnoughSamples(PlayRecord.Actor.Player, 1, MinSamplesPerSlot)
            && ctx.playRecord.HasEnoughSamples(PlayRecord.Actor.Player, 2, MinSamplesPerSlot);
    }

    /// <summary>
    /// 슬롯별 카운터 가중치 적용
    /// </summary>
    private static void ApplyCounterWeightsForAllSlots(TurnContext ctx)
    {
        for (int slot = 0; slot < 3; slot++)
        {
            Dominant dominant = GetDominantPlayerAction(ctx, slot);

            // 불명확하면 해당 슬롯만 Stage3로 fallback
            if (dominant == Dominant.None)
            {
                ApplyStage3ToSlot(ctx, slot);
                continue;
            }

            // 카운터 규칙 (RPS)
            // Player Attack -> AI Defense
            // Player Defense -> AI Heal
            // Player Heal -> AI Attack
            switch (dominant)
            {
                case Dominant.Attack:
                    SetSlotWeights(ctx, slot, attack: 1f, defense: 3f, heal: 1f);
                    break;
                case Dominant.Defense:
                    SetSlotWeights(ctx, slot, attack: 1f, defense: 1f, heal: 3f);
                    break;
                case Dominant.Heal:
                    SetSlotWeights(ctx, slot, attack: 3f, defense: 1f, heal: 1f);
                    break;
            }
        }
    }

    /// <summary>
    /// 특정 슬롯에서 플레이어의 우세한 행동 유형을 반환
    /// </summary>
    private static Dominant GetDominantPlayerAction(TurnContext ctx, int slot)
    {
        if (ctx.playRecord == null) return Dominant.None;

        ctx.playRecord.GetTendency(PlayRecord.Actor.Player, slot, out float atk, out float def, out float heal);

        float max = atk;
        Dominant dominant = Dominant.Attack;

        if (def > max) { max = def; dominant = Dominant.Defense; }
        if (heal > max) { max = heal; dominant = Dominant.Heal; }

        // threshold 미만이면 “중점”으로 보기 어려움 -> Stage3 fallback
        if (max < DominantThreshold) return Dominant.None;

        return dominant;
    }

    /// <summary>
    /// Stage3 기준(HP 비교): 슬롯별로 동일한 패턴 세팅
    /// </summary>
    private static void ApplyStage3WeightsForAllSlots(TurnContext ctx)
    {
        for (int slot = 0; slot < 3; slot++)
            ApplyStage3ToSlot(ctx, slot);
    }

    // Stage3 기준(HP 비교): 슬롯별로 동일한 패턴 세팅
    private static void ApplyStage3ToSlot(TurnContext ctx, int slot)
    {
        int pHp = ctx.playerCharactor.GetHealth();
        int eHp = ctx.enemyCharactor.GetHealth();

        // AI HP가 낮으면 회복 중점, 동일이면 수비 중점, 높으면 공격 중점
        if (eHp < pHp) SetSlotWeights(ctx, slot, attack: 1f, defense: 1f, heal: 3f);
        else if (eHp == pHp) SetSlotWeights(ctx, slot, attack: 1f, defense: 3f, heal: 1f);
        else SetSlotWeights(ctx, slot, attack: 3f, defense: 1f, heal: 1f);
    }

    private static void SetSlotWeights(TurnContext ctx, int slot, float attack, float defense, float heal)
    {
        // TurnContext에 이미 존재하는 슬롯별 가중치 배열을 사용
        ctx.aiAttackWeightsBySlot[slot] = attack;
        ctx.aiDefenseWeightsBySlot[slot] = defense;
        ctx.aiHealWeightsBySlot[slot] = heal;
    }
}
