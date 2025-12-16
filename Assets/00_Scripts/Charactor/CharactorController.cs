using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ==================================================================
// 목적 : 캐릭터(플레이어, 적)의 상태 정보를 UI로 출력하는 클래스, Presentation 영역
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/16
// ==================================================================

public class CharactorController : MonoBehaviour
{
    # region Properties
    // 캐릭터 상태 정보를 가질 데이터 객체
    [SerializeField] private CharactorData charactorStatus;
    // 캐릭터 이름 출력 Text UI
    [SerializeField] TMP_Text charactorNameText;
    // 캐릭터 체력 출력 Text UI
    [SerializeField] TMP_Text healthText;
    #endregion

    // TODO 25/12/09 : 우선은 플레이어, 적 전부 직렬화로 지정된 값을 사용, 추후에 Addressable 등으로 불러오는 방식으로 변경 예정
    private void Start()
    {
        // 캐릭터 이름 및 체력 UI 초기화 및 설정
        healthText.text = charactorStatus.GetHealth().ToString();
        charactorNameText.text = charactorStatus.GetCharactorName();
    }

    // [25/12/16] 추가 : 캐릭터 체력 업데이트 메서드
    /// <summary>
    /// 캐릭터의 체력 UI를 현재 상태에 맞게 업데이트한다.
    /// </summary>
    public void UpdateHealthUI()
    {
        healthText.text = charactorStatus.GetHealth().ToString();
    }
}
