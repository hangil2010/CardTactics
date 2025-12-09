using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ==================================================================
// 목적 : 캐릭터(플레이어, 적)의 상태 정보를 관리하는 클래스, Domain 영역
// 생성 일자 : 25/12/09
// 최근 수정 일자 : 25/12/09
// ==================================================================


public class CharactorData : MonoBehaviour
{
    #region Properties
    // 캐릭터 이름
    [SerializeField] private string charactorName;
    // Getter/Setter는 별도의 메서드로 구현
    public string GetCharactorName() { return charactorName; }
    public void SetCharactorName(string value) { charactorName = value; }
    
    // 캐릭터 체력
    [SerializeField] private int health;
    // Getter/Setter는 별도의 메서드로 구현
    public int GetHealth() { return health; }
    public void SetHealth(int value) { health = value; }
    
    #endregion
}
