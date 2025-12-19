// ==================================================================
// 목적 : CardEffectData 기반으로 카드 효과를 실제 전투 값(HP/Guard)에 적용
// 생성 일자 : 25/12/15
// 최근 수정 일자 : 25/12/15
// ==================================================================

using UnityEngine;

/// <summary>
/// 카드 효과를 해석하여 전투 상태에 적용하는 실행기.
/// </summary>
public static class ActionCardExecutor
{
    /// <summary>
    /// ActionCardData가 참조하는 CardEffectData를 기반으로 효과를 적용한다.
    /// </summary>
    // 25/12/19 수정 : 회복 카드 기록 추가 추가
    public static void Execute(ActionCardData card, CharactorData self, CharactorData target, TurnContext ctx, bool isPlayer)
    {
        if (card == null || card.EffectData == null) return;

        switch (card.EffectData.Type)
        {
            case ActionCardEffectData.EffectType.Attack:
                ApplyDamage(target, card.EffectData.AttackValue);
                if (ctx != null)
                {
                    if (isPlayer) ctx.playerUsedAttackThisCycle = true;
                    else ctx.enemyUsedAttackThisCycle = true;
                }
                break;

            case ActionCardEffectData.EffectType.Defense:
                self.SetIsGuarding(true);
                break;

            case ActionCardEffectData.EffectType.Heal:
                if (ctx != null)
                {
                    if (isPlayer) ctx.playerUsedHealThisCycle = true;
                    else ctx.enemyUsedHealThisCycle = true;
                }
                break;
        }
    }

    private static void ApplyDamage(CharactorData target, int damage)
    {
        if (target.GetIsGuarding())
        {
            target.SetIsGuarding(false); // 1회 막고 해제
            return;
        }

        int nextHp = Mathf.Max(0, target.GetHealth() - damage);
        target.SetHealth(nextHp);
    }
}
