using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 전투 행동 카드(공격, 방어)의 기본 정보를 ScriptableObject로 관리, Domain 영역
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/15
// ==================================================================
// TODO : Resources 시스템 -> Addressable 시스템 전환하여 자원 관리 최적화 지표 수립
[CreateAssetMenu( fileName = "NewActionCard", menuName = "Card/Action Card", order = 1)]
public class ActionCardData : ScriptableObject
{
    /// <summary>
    /// 카드의 동작 타입 (공격 / 방어).
    /// </summary>
    public enum ActionType
    {
        Attack,
        Defense
    }

    [Header("기본 정보")]
    [SerializeField] private string cardName;
    [SerializeField] private ActionType actionType;

    [Header("효과 설명")]
    [TextArea]
    [SerializeField] private string effectDescription;
    
    // 25/12/15 추가 : 카드 효과 데이터
    [Header("효과 데이터")]
    [SerializeField] private ActionCardEffectData effectData;
    
    /// <summary> 카드 이름. </summary>
    public string CardName => cardName;

    /// <summary> 카드 타입 (공격/방어).</summary>
    public ActionType Type => actionType;

    /// <summary> 카드 효과에 대한 설명 텍스트.</summary>
    public string EffectDescription => effectDescription;

    /// <summary> 카드 효과 데이터. </summary>
    public ActionCardEffectData EffectData => effectData;
}
