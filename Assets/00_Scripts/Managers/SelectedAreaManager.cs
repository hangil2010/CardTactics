using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : 선택 영역(3칸)을 관리하는 매니저, 카드 데이터가 없어 우선 선택 순서에 따라 색상(R,G,B)을 부여
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/09
// ==================================================================

public class SelectedAreaManager : MonoBehaviour
{
    [SerializeField] private Image[] selectedSlotImages;   // First/Second/Third Selected Area 의 Image
    [SerializeField] private Color[] orderColors = { Color.red, Color.green, Color.blue };

    private int _currentIndex = 0;

    /// <summary>
    /// 아직 남은 색이 있다면 targetImage에 다음 색을 적용한다.
    /// </summary>
    public bool TryApplyNextColor(Image targetImage)
    {
        if (_currentIndex >= orderColors.Length)
        {
            return false; // 이미 3개 모두 선택된 상태
        }

        targetImage.color = orderColors[_currentIndex];
        _currentIndex++;
        return true;
    }
}
