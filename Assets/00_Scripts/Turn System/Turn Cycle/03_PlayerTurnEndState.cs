using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 종료 시 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

/// <summary>
/// 플레이어 턴을 종료하고 AI 턴으로 넘기기 위한 상태.
/// 필요 시 전투 결과 업데이트를 수행할 수 있다.
/// </summary>
public class PlayerTurnEndState : TurnStateBase
{
    public PlayerTurnEndState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = false;
        Debug.Log("플레이어 턴 종료");

        // [25/12/15] 수정: 플레이어 턴 종료 후 AI 즉시 결정 → 전투 사이클로 진입
        machine.ChangeState(new AiDecideState(ctx, machine));
    }
}