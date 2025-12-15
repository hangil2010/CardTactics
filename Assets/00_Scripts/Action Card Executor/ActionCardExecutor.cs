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
    public static void Execute(ActionCardData card, BattleUnit self, BattleUnit target)
    {
        if (card == null || card.EffectData == null) return;

        var effect = card.EffectData;

        // 효과 타입에 따라 분기 처리
        switch (effect.Type)
        {
            // 공격 행동일 때
            case ActionCardEffectData.EffectType.DealDamage:
                ApplyDamage(target, effect.Value);
                break;
            // 방어 행동일 때
            case ActionCardEffectData.EffectType.Guard:
                self.IsGuarding = true;
                break;
        }
    }

    private static void ApplyDamage(BattleUnit target, int damage)
    {
        if (target.IsGuarding)
        {
            // 방어 자세로 1회 막기
            target.IsGuarding = false;
            return;
        }

        target.Hp = Mathf.Max(0, target.Hp - damage);
    }
}
