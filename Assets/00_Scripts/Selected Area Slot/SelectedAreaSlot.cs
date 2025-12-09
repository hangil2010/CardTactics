using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ==================================================================
// 목적 : 드래그된 카드를 드롭받아, 선택 영역에 색을 적용하는 슬롯
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/09
// ==================================================================

public class SelectedAreaSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SelectedAreaManager manager;
    [SerializeField] private Image slotImage;

    private bool _isSelected = false;

    /// <summary>
    /// 카드를 드롭했을 때 호출된다.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (_isSelected) return; // 이미 선택된 슬롯이면 무시

        // 드롭된 오브젝트가 DraggableCard인지 확인
        var draggableCard = eventData.pointerDrag?.GetComponent<DraggableCard>();
        if (draggableCard == null) return;

        // 선택 순서에 따라 색 적용 시도
        if (manager.TryApplyNextColor(slotImage))
        {
            _isSelected = true;
            Debug.Log($"{gameObject.name} 슬롯 선택됨");
        }
    }
}
