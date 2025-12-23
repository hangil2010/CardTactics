using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ==================================================================
// 목적 : UI 카드를 마우스로 드래그하고, 드래그 종료 시 원래 위치로 되돌리는 기능 제공
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/10
// ==================================================================

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;                // UI Canvas
    [SerializeField] private RectTransform cardAreaPanel;  // Card Area Panel (기준 Rect)

    // [25/12/10] 수정: 이 카드가 표시하는 ActionCardData에 접근하기 위해 ActionCardView 참조 추가
    [Header("카드 데이터")]
    [SerializeField] private ActionCardView actionCardView;

    private RectTransform _rectTransform;
    private Vector2 _originAnchoredPos;    // Card Area Panel 기준 초기 위치
    private Vector2 _pointerOffset;        // 마우스와 카드 기준점 사이 오프셋

    // [25/12/10] 수정: SelectedAreaSlot 등에서 사용할 수 있도록 카드 데이터를 외부에 노출
    /// <summary>이 드래그 카드가 들고 있는 ActionCardData.</summary>
    public ActionCardData CardData => actionCardView != null ? actionCardView.Data : null;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // 안 넣었으면 부모를 Card Area Panel로 사용
        if (cardAreaPanel == null)
        {
            cardAreaPanel = _rectTransform.parent as RectTransform;
        }

        // [25/12/10] 수정: 같은 오브젝트에 붙은 ActionCardView를 자동으로 참조
        if (actionCardView == null)
        {
            actionCardView = GetComponent<ActionCardView>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Card Area Panel 기준 현재 위치를 저장
        _originAnchoredPos = _rectTransform.anchoredPosition;

        if (canvas == null || cardAreaPanel == null) return;

        // 마우스 위치를 Card Area Panel 기준 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardAreaPanel,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint))
        {
            // 현재 카드 위치 - 마우스 위치 = 오프셋
            _pointerOffset = _rectTransform.anchoredPosition - localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null || cardAreaPanel == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardAreaPanel,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint))
        {
            // Panel 기준 마우스 위치 + 오프셋
            _rectTransform.anchoredPosition = localPoint + _pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Panel 기준 초기 위치로 되돌림
        _rectTransform.anchoredPosition = _originAnchoredPos;
    }
}
