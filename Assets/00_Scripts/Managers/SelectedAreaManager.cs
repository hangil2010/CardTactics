using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : 선택 영역(3칸)을 관리하는 매니저, 카드 데이터가 없어 우선 선택 순서에 따라 색상(R,G,B)을 부여
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/30
// ==================================================================

public class SelectedAreaManager : MonoBehaviour
{
    [SerializeField] private Image[] selectedSlotImages;   // First/Second/Third Selected Area 의 Image
    // [25/12/30] 수정 : 카드 타입별 색상 지정
    [Header("Card Type Colors")]
    [SerializeField] private Color attackColor = Color.red;
    [SerializeField] private Color defenseColor = Color.blue;
    [SerializeField] private Color healColor = Color.green;
    
    // [25/12/10] 수정: 선택된 카드 데이터를 저장하기 위한 배열 추가
    [SerializeField] private ActionCardData[] _selectedCards = new ActionCardData[3];

    /// <summary>선택된 카드 배열(ReadOnly).</summary>
    public IReadOnlyList<ActionCardData> SelectedCards => _selectedCards;

    private int _currentIndex = 0;

    // 시작 시 선택 영역을 초기화한다.
    private void Start()
    {
        ResetSelection();
    }

    /// <summary>
    /// 다음 선택 인덱스에 카드를 등록하고, 카드 타입에 맞는 색상을 targetImage에 적용한다.
    /// </summary>
    // [25/12/30] 수정 : 카드 타입에 따른 색상 적용 시도
    public bool TryRegisterCardAndApplyColor(ActionCardData cardData, Image targetImage, out int appliedIndex)
    {
        appliedIndex = -1;

        if (cardData == null || targetImage == null)
            return false;

        if (_currentIndex >= _selectedCards.Length)
            return false;

        // 카드 등록 및 색상 적용
        appliedIndex = _currentIndex;
        _selectedCards[appliedIndex] = cardData;
        // 카드 타입에 따른 색상 적용
        targetImage.color = GetColorByCardType(cardData);

        _currentIndex++;
        Debug.Log($"[SelectedAreaManager] {appliedIndex + 1}번째 선택 카드 등록: {cardData.CardName}");
        return true;
    }
    // [25/12/30] 추가: 카드 타입에 따른 색상 반환 메서드
    private Color GetColorByCardType(ActionCardData cardData)
    {

        if (cardData == null)
            return Color.white;

        var effectData = cardData.EffectData; 
        if (effectData == null)
            return Color.white;

        // 카드 타입에 따른 색상 반환
        switch (effectData.Type)
        {
            case ActionCardEffectData.EffectType.Attack:  return attackColor;
            case ActionCardEffectData.EffectType.Defense: return defenseColor;
            case ActionCardEffectData.EffectType.Heal:    return healColor;
            default:                                      return Color.white;
        }
    }

    // [25/12/16] 추가: 선택된 행동 카드 초기화 기능 추가
    /// <summary>
    /// 선택 영역을 초기화한다.
    /// </summary>
    public void ResetSelection()
    {
        _currentIndex = 0;

        // 선택된 카드 데이터 초기화
        for (int i = 0; i < _selectedCards.Length; i++)
            _selectedCards[i] = null;
            
        // 슬롯 이미지 초기화
        for (int i = 0; i < selectedSlotImages.Length; i++)
        {
            var img = selectedSlotImages[i];
            if (img == null) continue;

            img.color = Color.white;

            var slot = img.GetComponent<SelectedAreaSlot>();
            if (slot != null)
                slot.ResetSlot();
        }
        Debug.Log("[SelectedAreaManager] 선택 슬롯 초기화 완료");
    }
}
