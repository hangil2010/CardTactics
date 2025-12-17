using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : UI 입력과 상태 머신을 연결하여 턴 진행을 제어하는 프레젠테이션 레이어 컨트롤러
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/17
// ==================================================================

/// <summary>
/// UI 버튼과 텍스트를 통해 턴 상태 머신과 상호작용하는 컨트롤러.
/// 프레젠테이션 레이어에서 Domain의 턴 로직을 호출한다.
/// </summary>
public class TurnController : MonoBehaviour
{
    #region Inspector Fields
    [Header("UI References")]
    [SerializeField] private TMP_Text turnStateText;
    [SerializeField] private Button turnEndButton;
    [Header("Battle References")]
    /// <summary> 플레이어가 선택한 카드 조회용 매니저 </summary>
    [SerializeField] private SelectedAreaManager selectedAreaManager;

    // 25/12/15 추가 CharactorData 참조 주입
    [SerializeField] private CharactorData playerCharactor;
    [SerializeField] private CharactorData enemyCharactor;

    // 25/12/16 추가 : CharactorController 참조 주입
    [Header("Charactor UI References")]
    [SerializeField] private CharactorController playerCharactorUI;
    [SerializeField] private CharactorController enemyCharactorUI;

    [Header("AI Weight Config")]
    [SerializeField] private AIWeightData defaultAiWeights;
    [SerializeField] private AIWeightOverrideProvider aiWeightOverride;

    #endregion

    private TurnStateMachine _machine;
    private TurnContext _context;

    /// <summary>
    /// UI 참조를 TurnContext에 할당하고 버튼 이벤트를 등록한다.
    /// </summary>
    private void Awake()
    {
        // [25/12/17] 추가 기본 AI 가중치 로드
        float atk = defaultAiWeights != null ? defaultAiWeights.attackWeight : 1f;
        float def = defaultAiWeights != null ? defaultAiWeights.defenseWeight : 1f;

        // [25/12/17] 오버라이드 컴포넌트가 활성화된 경우 가중치 덮어쓰기
        if (aiWeightOverride != null && aiWeightOverride.useOverride)
        {
            atk = aiWeightOverride.attackWeight;
            def = aiWeightOverride.defenseWeight;
        }


        _context = new TurnContext
        {
            turnStateText = turnStateText,
            turnEndButton = turnEndButton,

            // [25/12/15] 수정: 전투 사이클에서 사용할 참조 주입
            selectedAreaManager = selectedAreaManager,
            aiPlannedCards = new ActionCardData[3],

            // [25/12/15] 수정: 효과 적용을 위한 유닛 데이터 주입
            playerCharactor = playerCharactor,
            enemyCharactor = enemyCharactor,

            // [25/12/16] 추가 : 캐릭터 UI 주입
            playerCharactorUI = playerCharactorUI,
            enemyCharactorUI = enemyCharactorUI,

            // [25/12/17] 추가 : AI 행동 가중치 주입
            aiAttackWeight = atk,
            aiDefenseWeight = def,
        };

        _machine = new TurnStateMachine();

        //버튼 이벤트 연결
        turnEndButton.onClick.AddListener(OnTurnEndButtonClicked);
        // [25/12/17] 디버그용 가중치 출력
        Debug.Log(
        $"[AI Init] AttackWeight={_context.aiAttackWeight}, " +
        $"DefenseWeight={_context.aiDefenseWeight}, " +
        $"OverrideUsed={aiWeightOverride != null && aiWeightOverride.useOverride}"
        );
    }

    /// <summary>
    /// 게임 시작 시 플레이어 턴을 최초 진입 상태로 설정한다.
    /// </summary>
    private void Start()
    {
        //게임 시작 시 플레이어 턴부터 시작
        _machine.ChangeState(new PlayerTurnStartState(_context, _machine));
    }

    /// <summary>
    /// 매 프레임 상태 머신 Tick을 호출한다.
    /// </summary>
    private void Update()
    {
        _machine.Tick();
    }

    /// <summary>
    /// 플레이어의 '턴 종료' 버튼 입력을 현재 상태에 전달한다.
    /// </summary>
    private void OnTurnEndButtonClicked()
    {
        _machine.OnTurnEndButtonPressed();
    }
}