using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 시작의 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

/// <summary>
/// 플레이어 턴의 시작을 처리하는 상태.
/// 초기 설정 후 PlayerDecideActionState로 즉시 전환된다.
/// </summary>
public class PlayerTurnStartState : TurnStateBase
{
    public PlayerTurnStartState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = false;
        // [25/12/30] 추가 : 턴 카운트 UI 갱신
        ctx.turnCountText.text = $"Turn {ctx.currentTurnCount}";
        Debug.Log("플레이어 턴 시작");
        machine.ChangeState(new PlayerDecideActionState(ctx, machine));
    }
}