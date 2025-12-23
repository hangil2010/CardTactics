using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
// ==================================================================
// 목적 : Resources/Card 에서 모든 행동 카드 데이터를 로드하고, 타입별로 관리하는 매니저, Domain/Infrastructure 영역
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/23
// ==================================================================
// [25/12/23] 수정 : Resources -> Addressables 전환 진행, 기존 Resources 코드는 Obsolete 처리
public class ActionCardDataManager : MonoBehaviour
{
    private static ActionCardDataManager _instance;
    /// <summary>전역 접근용 인스턴스.</summary>
    public static ActionCardDataManager Instance => _instance;

    [Header("Resources 설정(Obsolete)")]
    
    [SerializeField, Obsolete("Resources 시스템")] private string resourcesPath = "Card";  // Resources/Card

    // [25/12/23] 추가 : Addressables 설정
    [Header("Addressables 설정")]
    [SerializeField] private string cardDataLabel = "card_data";

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

    // [25/12/23] 추가 : Addressables 로드 완료 여부
    public bool IsReady { get; private set; }
    // [25/12/23] 추가 : Addressables 비동기 로드 핸들
    private AsyncOperationHandle<IList<ActionCardData>> _loadHandle;

    // [25/12/23] 추가 : 카드 로드 완료 이벤트
    public event Action OnCardsLoaded;
    // 카드 데이터를 가장 우선 불러오기 위해 Awake 대신 OnEnable 사용

    private void Awake()
    {
        // 싱글톤 초기화
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        // 중복 로딩 방지
        if (IsReady) return;
        // [25/12/23] 수정 : Addressables 기반 카드 데이터 로드 시작
        StartCoroutine(LoadAllCardsFromAddressables());

        // [25/12/23] 수정 : Resources 로드 방식 주석 처리
        //LoadAllCardsFromResources();
    }

    private void OnDisable()
    {
        if (_loadHandle.IsValid())
            Addressables.Release(_loadHandle);
    }
    /// <summary>
    /// Resources/Card 경로에서 모든 ActionCardData를 로드하고, 타입별로 분류한다.
    /// </summary>
    // [25/12/23] Obsolete 처리
    [Obsolete("Resources 시스템에서 Addressable 시스템으로 이동")]
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
        Debug.Log($"[ActionCardDataManager] Loaded Cards(Resources): Total={allCards.Count}, " +
                  $"Attack={attackCards.Count}, Defense={DefenseCards.Count}" +
                  $", Heal={healCards.Count}");
    }

    /// <summary>
    /// Addressables 라벨로 모든 ActionCardData를 비동기로 로드하고, 타입별로 분류한다.
    /// </summary>
    // [25/12/23] 추가 : Addressables 시스템 기반 로드
    private IEnumerator LoadAllCardsFromAddressables()
    {
        IsReady = false;

        allCards.Clear();
        attackCards.Clear();
        defenseCards.Clear();
        healCards.Clear();

        _loadHandle = Addressables.LoadAssetsAsync<ActionCardData>(cardDataLabel, null);
        yield return _loadHandle;

        if (_loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[ActionCardDataManager] Failed to load ActionCardData by label: {cardDataLabel}");
            yield break;
        }

        allCards.AddRange(_loadHandle.Result);

        attackCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Attack));
        defenseCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Defense));
        healCards.AddRange(allCards.Where(c => c.Type == ActionCardData.ActionType.Heal));

        IsReady = true;

        // [25/12/23] 추가 : 카드 로드 완료 이벤트 호출
        // Issue#15의 수정 사항
        OnCardsLoaded?.Invoke();

        Debug.Log($"[ActionCardDataManager] Loaded Cards(Addressables): Total={allCards.Count}, " +
                  $"Attack={attackCards.Count}, Defense={defenseCards.Count}, Heal={healCards.Count}");
    }

    /// <summary>
    /// 비활성화 시 Addressables 로드 핸들 해제.
    /// </summary>
    

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