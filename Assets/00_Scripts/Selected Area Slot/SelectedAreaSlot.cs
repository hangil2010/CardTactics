using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ==================================================================
// 목적 : 드래그된 카드를 드롭받아, 선택 영역에 색을 적용하는 슬롯
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/16
// ==================================================================

public class SelectedAreaSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SelectedAreaManager manager;
    [SerializeField] private Image slotImage;

    // [25/12/10] 수정: 슬롯 자체에서도 카드 이름/효과를 표시하기 위해 ActionCardView 추가
    [SerializeField] private ActionCardView slotCardView;

    private bool _isSelected = false;

    // [25/12/10] 수정: 슬롯이 담고 있는 ActionCardData 저장 변수 추가
    private ActionCardData _selectedCard;

    /// <summary>이 슬롯에 최종적으로 선택된 카드 데이터.</summary>
    public ActionCardData SelectedCard => _selectedCard;

    private void Awake()
    {
        // [25/12/10] 수정: slotCardView가 비어 있으면 동일 오브젝트에서 자동으로 찾기
        if (slotCardView == null)
        {
            slotCardView = GetComponent<ActionCardView>();
        }
    }

    /// <summary>
    /// 카드를 드롭했을 때 호출된다.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (_isSelected) return;

        // [25/12/10] 수정: 드롭된 카드에서 ActionCardData를 가져오도록 변경
        var draggableCard = eventData.pointerDrag?.GetComponent<DraggableCard>();
        if (draggableCard == null) return;

        var cardData = draggableCard.CardData;
        if (cardData == null)
        {
            Debug.LogWarning("[SelectedAreaSlot] 드롭된 카드에 ActionCardData가 없습니다.");
            return;
        }

        // [25/12/10] 수정: 색 적용 후 선택 순서 index를 받아 카드 등록하도록 확장
        if (manager.TryApplyNextColor(slotImage, out int index))
        {
            _isSelected = true;
            _selectedCard = cardData;

            // 매니저에 카드 데이터 등록
            manager.RegisterSelectedCard(index, _selectedCard);

            // [25/12/10] 수정: 슬롯 UI에 선택된 카드 데이터 표시
            if (slotCardView != null)
            {
                slotCardView.SetData(_selectedCard);
            }

            Debug.Log($"{gameObject.name} 슬롯 선택됨: {_selectedCard.CardName} (순번 {index + 1})");
        }
    }

    // [25/12/16] 추가: 슬롯 초기화 기능 추가
    /// <summary>
    /// 해당 슬롯을 초기화
    /// </summary>
    public void ResetSlot()
    {
        _isSelected = false;
        _selectedCard = null;

        // 슬롯에 표시된 카드 UI도 비우기
        if (slotCardView != null)
            slotCardView.SetData(null);
    }
}
