using UnityEngine;

// ==================================================================
// 목적 : 씬 단위에서 AI 행동 가중치를 임시로 덮어쓰기 위한 디버그/튜닝용 컴포넌트
// 생성 일자 : 2025/12/17
// 최근 수정 일자 : 2025/12/17
// ==================================================================

/// <summary>
/// ScriptableObject로 정의된 AI 가중치를 씬 단위에서 덮어쓰기 위해 사용하는 Provider.
/// </summary>
public class AIWeightOverrideProvider : MonoBehaviour
{
    [Header("Override Toggle")]
    public bool useOverride = false;

    [Header("Override Weights")]
    [Min(0f)] public float attackWeight = 1f;
    [Min(0f)] public float defenseWeight = 1f;
}
