using UnityEngine;

// ==================================================================
// 목적 : 플레이어 행동 결정 턴의 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

/// <summary>
/// 플레이어가 행동을 결정하는 단계.
/// UI 상에서 턴 종료 버튼을 눌러 다음 상태로 넘어갈 수 있다.
/// </summary>
public class PlayerDecideActionState : TurnStateBase
{
    public PlayerDecideActionState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = true;
        Debug.Log("플레이어 행동 결정 단계");
    }

    public override void OnTurnEndButtonPressed()
    {
        // 여기에서 나중에 '선택한 카드가 있는지 검증' 같은 것 추가 가능
        machine.ChangeState(new PlayerTurnEndState(ctx, machine));
    }
}