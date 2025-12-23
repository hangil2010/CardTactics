using UnityEngine;

// ==================================================================
// 목적 : AI 행동 선택 시 사용할 공격/방어 타입 가중치 기본값을 정의하는 데이터 에셋
// 생성 일자 : 2025/12/17
// 최근 수정 일자 : 2025/12/19
// ==================================================================

/// <summary>
/// AI가 행동 카드를 선택할 때 사용할 공격/방어 타입별 기본 가중치를 정의하는 ScriptableObject.
/// </summary>
[CreateAssetMenu(menuName = "CardTactics/AI/Type Weights")]
public class AIWeightData : ScriptableObject
{
    [Min(0f)] public float attackWeight = 1f;
    [Min(0f)] public float defenseWeight = 1f;
    // [25/12/19] 추가 : 회복 행동 가중치
    [Min(0f)] public float healWeight = 1f;
}
