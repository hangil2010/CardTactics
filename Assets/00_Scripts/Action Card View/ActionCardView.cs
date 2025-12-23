using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : ScriptableObject 카드 데이터를 받아 UI에 카드 이름/효과를 표시하는 뷰, Presentation 영역
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/16
// ==================================================================

public class ActionCardView : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text effectDescriptionText;
    [SerializeField] private Image backgroundImage; // 필요 없으면 Inspector에서 비워도 됨

    private ActionCardData _data;

    /// <summary>이 카드 UI에 바인딩된 데이터.</summary>
    public ActionCardData Data => _data;

    /// <summary>
    /// 지정한 카드 데이터를 이 UI에 바인딩하고, 텍스트를 갱신한다.
    /// </summary>
    public void SetData(ActionCardData data)
    {
        _data = data;

        if (_data == null)
        {
            // [25/12/16] 수정: 데이터가 없으면 카드 설명란에 "No Card" 표시
            cardNameText.text = string.Empty;
            effectDescriptionText.text = "No Card";
            return;
        }

        cardNameText.text = _data.CardName;
        effectDescriptionText.text = _data.EffectDescription;

        // 타입에 따라 배경색 등을 바꾸고 싶다면 여기서 처리 가능
        if (backgroundImage != null)
        {
            switch (_data.Type)
            {
                case ActionCardData.ActionType.Attack:
                    // 공격 카드임을 시각적으로 구분하고 싶으면 색상 등 지정
                    // backgroundImage.color = Color.red;
                    break;
                case ActionCardData.ActionType.Defense:
                    // backgroundImage.color = Color.blue;
                    break;
            }
        }
    }
}
