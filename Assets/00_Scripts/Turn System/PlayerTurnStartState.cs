using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 시작/행동 결정/종료 및 AI 턴 진행 흐름을 관리하는 턴 상태 클래스 모음
// 생성 일자 : 12/08
// 최근 수정 일자 : 12/08
// ==================================================================


#region PlayerTurnStartState
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
        Debug.Log("플레이어 턴 시작");
        machine.ChangeState(new PlayerDecideActionState(ctx, machine));
    }
}
#endregion

#region PlayerDecideActionState
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
#endregion

#region PlayerTurnEndState
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
        Debug.Log("플레이어 턴 종료, 결과 업데이트(임시)");
        // 결과 업데이트 후 바로 AI 턴으로
        machine.ChangeState(new AiTurnState(ctx, machine));
    }
}
#endregion

#region AiTurnState
/// <summary>
/// AI의 행동 결정, 실행, 결과 업데이트를 처리하는 상태.
/// 현재는 더미 로직이며 즉시 플레이어 턴으로 복귀한다.
/// </summary>
public class AiTurnState : TurnStateBase
{
    public AiTurnState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        Debug.Log("AI 행동 결정");
        // TODO: 여기서 AI 카드 선택 로직(지금은 더미)
        Debug.Log("AI 행동 플레이");
        Debug.Log("행동 결과 업데이트");
        Debug.Log("AI 턴 종료");
        // AI는 즉시 처리 후 다시 플레이어 턴 시작
        machine.ChangeState(new PlayerTurnStartState(ctx, machine));
    }
}
#endregion