using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : UI 입력과 상태 머신을 연결하여 턴 진행을 제어하는 프레젠테이션 레이어 컨트롤러
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/15
// ==================================================================

/// <summary>
/// UI 버튼과 텍스트를 통해 턴 상태 머신과 상호작용하는 컨트롤러.
/// 프레젠테이션 레이어에서 Domain의 턴 로직을 호출한다.
/// </summary>
public class TurnController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text turnStateText;
    [SerializeField] private Button turnEndButton;
    [Header("Battle References")]
    /// <summary> 플레이어가 선택한 카드 조회용 매니저 </summary>
    [SerializeField] private SelectedAreaManager selectedAreaManager;

    // 25/12/15 추가 CharactorData 참조 주입
    [SerializeField] private CharactorData playerCharactor;
    [SerializeField] private CharactorData enemyCharactor;

    private TurnStateMachine _machine;
    private TurnContext _context;

    /// <summary>
    /// UI 참조를 TurnContext에 할당하고 버튼 이벤트를 등록한다.
    /// </summary>
    private void Awake()
    {

        _context = new TurnContext
        {
            turnStateText = turnStateText,
            turnEndButton = turnEndButton,

            // [25/12/15] 수정: 전투 사이클에서 사용할 참조 주입
            selectedAreaManager = selectedAreaManager,
            aiPlannedCards = new ActionCardData[3],

            // [25/12/15] 수정: 효과 적용을 위한 유닛 데이터 주입
            playerCharactor = playerCharactor,
            enemyCharactor = enemyCharactor
        };

        _machine = new TurnStateMachine();

        //버튼 이벤트 연결
        turnEndButton.onClick.AddListener(OnTurnEndButtonClicked);
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