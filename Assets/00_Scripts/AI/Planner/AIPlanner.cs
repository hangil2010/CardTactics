using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : AI의 이번 전투 사이클 행동 카드를 결정하는 로직을 제공
// 생성 일자 : 2025/12/17
// 최근 수정 일자 : 2025/12/21
// ==================================================================

/// <summary>
/// 가중치 기반 랜덤 방식을 통해 AI의 행동 카드 선택을 담당하는 정적 유틸리티 클래스.
/// </summary>
public static class AiPlanner
{
    /// <summary>
    /// 이번 사이클에서 AI가 사용할 최대 3장의 행동 카드를 결정한다.
    /// </summary>
    public static void Plan3(TurnContext ctx)
    {
        if (ctx.aiPlannedCards == null || ctx.aiPlannedCards.Length != 3)
            ctx.aiPlannedCards = new ActionCardData[3];

        var mgr = ActionCardDataManager.Instance;
        for (int slot = 0; slot < 3; slot++)
        {
            var picked = PickOneForSlot(ctx, mgr, slot);
            ctx.aiPlannedCards[slot] = picked;
        }
    }
    /// <summary>
    // 이번 사이클에서 AI가 사용할 하나의 행동 카드를 결정한다.
    /// </summary>
    private static ActionCardData PickOneForSlot(TurnContext ctx, ActionCardDataManager mgr, int slotIndex)
    {
        if (mgr == null) return null;

        float wA = Mathf.Max(0f, ctx.GetAiAttackWeight(slotIndex));
        float wD = Mathf.Max(0f, ctx.GetAiDefenseWeight(slotIndex));
        float wH = Mathf.Max(0f, ctx.GetAiHealWeight(slotIndex));

        float total = wA + wD + wH;
        if (total <= 0f) return null;

        float r = Random.value * total;

        if (r < wA)
            return PickFrom(mgr.AttackCards);
        r -= wA;

        if (r < wD)
            return PickFrom(mgr.DefenseCards);
        r -= wD;

        return PickFrom(mgr.HealCards);
    }

    private static ActionCardData PickFrom(System.Collections.Generic.IReadOnlyList<ActionCardData> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 공격/방어 가중치를 기반으로 하나의 행동 카드를 선택한다.
    /// </summary>
    private static ActionCardData PickOne(TurnContext ctx, ActionCardDataManager mgr)
    {
        if (mgr == null) return null;

        float wA = Mathf.Max(0f, ctx.aiAttackWeight);
        float wD = Mathf.Max(0f, ctx.aiDefenseWeight);
        float wH = Mathf.Max(0f, ctx.aiHealWeight);
        float total = wA + wD + wH;
        if (total <= 0f) return null;

        float r = Random.value * total;

        ActionCardData.ActionType type;
        if (r < wA)
            type = ActionCardData.ActionType.Attack;
        else if (r < wA + wD)
            type = ActionCardData.ActionType.Defense;
        else
            type = ActionCardData.ActionType.Heal;

        var list = GetListByType(mgr, type);

        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 행동 카드 매니저에서 특정 타입의 카드 리스트를 반환한다.
    /// </summary>
    private static IReadOnlyList<ActionCardData> GetListByType( ActionCardDataManager mgr, ActionCardData.ActionType type)
    {
        return type switch
        {
            ActionCardData.ActionType.Attack => mgr.AttackCards,
            ActionCardData.ActionType.Defense => mgr.DefenseCards,
            ActionCardData.ActionType.Heal => mgr.HealCards,
            _ => null
        };
    }
}
