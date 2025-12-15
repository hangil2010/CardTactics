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
    public static void Execute(ActionCardData card, CharactorData self, CharactorData target)
    {
        if (card == null || card.EffectData == null) return;

        switch (card.EffectData.Type)
        {
            case ActionCardEffectData.EffectType.Attack:
                ApplyDamage(target, card.EffectData.Value);
                break;

            case ActionCardEffectData.EffectType.Defense:
                self.SetIsGuarding(true);
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
