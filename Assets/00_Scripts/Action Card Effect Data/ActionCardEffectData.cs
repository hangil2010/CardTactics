using UnityEngine;

// ==================================================================
// 목적 : 행동 카드의 효과를 ScriptableObject로 분리하여 관리 (Resources/CardEffectData)
// 생성 일자 : 25/12/15
// 최근 수정 일자 : 25/12/15
// ==================================================================

/// <summary>
/// 카드 효과 데이터. 카드(ActionCardData)에서 참조하여 실제 효과를 분리 관리한다.
/// </summary>
[CreateAssetMenu(
    fileName = "NewCardEffect",
    menuName = "Card/Effect Data",
    order = 1)]
public class ActionCardEffectData : ScriptableObject
{
    /// <summary>
    /// 효과 종류. 현재는 기초 플레이용으로 2개만 제공한다.
    /// </summary>
    public enum EffectType
    {
        Attack,   // 대상에게 피해
        Defense         // 방어 자세(다음 공격 1회 방어)
    }

    [Header("기본 정보")]
    [SerializeField] private string effectName;
    [SerializeField] private EffectType effectType;

    [Header("파라미터")]
    [SerializeField] private int value = 1; // DealDamage의 데미지 값 등 (Guard는 현재 미사용)

    public string EffectName => effectName;
    public EffectType Type => effectType;
    public int Value => value;
}
