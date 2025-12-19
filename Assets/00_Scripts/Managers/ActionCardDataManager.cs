using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ==================================================================
// 목적 : Resources/Card 에서 모든 행동 카드 데이터를 로드하고, 타입별로 관리하는 매니저, Domain/Infrastructure 영역
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/19
// ==================================================================

public class ActionCardDataManager : MonoBehaviour
{
    private static ActionCardDataManager _instance;
    /// <summary>전역 접근용 인스턴스.</summary>
    public static ActionCardDataManager Instance => _instance;

    [Header("Resources 설정(임시)")]
    [SerializeField] private string resourcesPath = "Card";  // Resources/Card

    [Header("디버그용 조회 리스트")]
    [SerializeField] private List<ActionCardData> allCards = new List<ActionCardData>();
    [SerializeField] private List<ActionCardData> attackCards = new List<ActionCardData>();
    [SerializeField] private List<ActionCardData> defenseCards = new List<ActionCardData>();
    // [25/12/19] 추가 : 회복 카드 리스트
    [SerializeField] private List<ActionCardData> healCards = new List<ActionCardData>();

    /// <summary>모든 카드 목록(ReadOnly).</summary>
    public IReadOnlyList<ActionCardData> AllCards => allCards;
    /// <summary>공격 카드 목록(ReadOnly).</summary>
    public IReadOnlyList<ActionCardData> AttackCards => attackCards;
    /// <summary>방어 카드 목록(ReadOnly).</summary>
    public IReadOnlyList<ActionCardData> DefenseCards => defenseCards;
    /// <summary>회복 카드 목록(ReadOnly).</summary>
    public IReadOnlyList<ActionCardData> HealCards => healCards;

    // 카드 데이터를 가장 우선 불러오기 위해 Awake 대신 OnEnable 사용
    private void OnEnable()
    {
        // 싱글톤 초기화
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllCardsFromResources();
    }

    /// <summary>
    /// Resources/Card 경로에서 모든 ActionCardData를 로드하고, 타입별로 분류한다.
    /// </summary>
    private void LoadAllCardsFromResources()
    {
        allCards.Clear();
        attackCards.Clear();
        defenseCards.Clear();
        // [25/12/19] 추가 : 회복 카드 리스트 초기화
        healCards.Clear();

        var loadedCards = Resources.LoadAll<ActionCardData>(resourcesPath);
        allCards.AddRange(loadedCards);

        attackCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Attack));
        defenseCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Defense));
        // [25/12/19] 추가 : 회복 카드 분류
        healCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Heal));
        Debug.Log($"[ActionCardDataManager] Loaded Cards: Total={allCards.Count}, " +
                  $"Attack={attackCards.Count}, Defense={DefenseCards.Count}" +
                  $", Heal={healCards.Count}");
    }

    /// <summary>
    /// 지정한 타입(공격/방어)의 첫 번째 카드를 반환한다. 없으면 null.
    /// </summary>
    public ActionCardData GetFirstCard(ActionCardData.ActionType type)
    {
        switch (type)
        {
            case ActionCardData.ActionType.Attack:
                return attackCards.Count > 0 ? attackCards[0] : null;
            case ActionCardData.ActionType.Defense:
                return defenseCards.Count > 0 ? defenseCards[0] : null;
            // [25/12/19] 추가 : 회복 카드 반환
            case ActionCardData.ActionType.Heal:
                return healCards.Count > 0 ? healCards[0] : null;
            default:
                return null;
        }
    }
}
