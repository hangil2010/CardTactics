using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : UI 입력과 상태 머신을 연결하여 턴 진행을 제어하는 프레젠테이션 레이어 컨트롤러
// 생성 일자 : 12/08
// 최근 수정 일자 : 12/08
// ==================================================================

public class TurnController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text turnStateText;
    [SerializeField] private Button turnEndButton;

    private TurnStateMachine _machine;
    private TurnContext _context;

    private void Awake()
    {
        _context = new TurnContext
        {
            turnStateText = turnStateText,
            turnEndButton = turnEndButton
        };

        _machine = new TurnStateMachine();

        //버튼 이벤트 연결
        turnEndButton.onClick.AddListener(OnTurnEndButtonClicked);
    }

    private void Start()
    {
        //게임 시작 시 플레이어 턴부터 시작
        _machine.ChangeState(new PlayerTurnStartState(_context, _machine));
    }

    private void Update()
    {
        _machine.Tick();
    }

    private void OnTurnEndButtonClicked()
    {
        _machine.OnTurnEndButtonPressed();
    }
}