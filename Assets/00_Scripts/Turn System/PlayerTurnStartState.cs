using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 시작/행동 결정/종료 및 AI 턴 진행 흐름을 관리하는 턴 상태 클래스 모음
// 생성 일자 : 12/08
// 최근 수정 일자 : 12/08
// ==================================================================

// 1. 플레이어 턴 시작 상태
public class PlayerTurnStartState : TurnStateBase
{
    public PlayerTurnStartState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = false;
        //ctx.turnStateText.text = "플레이어 턴 시작";
        Debug.Log("플레이어 턴 시작");
        machine.ChangeState(new PlayerDecideActionState(ctx, machine));
    }
}
// 2. 플레이어 행동 결정 상태
public class PlayerDecideActionState : TurnStateBase
{
    public PlayerDecideActionState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        //ctx.turnStateText.text = "자신 턴 행동 결정";
        ctx.turnEndButton.interactable = true;
        Debug.Log("플레이어 행동 결정 단계");
    }

    public override void OnTurnEndButtonPressed()
    {
        // 여기에서 나중에 '선택한 카드가 있는지 검증' 같은 것 추가 가능
        machine.ChangeState(new PlayerTurnEndState(ctx, machine));
    }
}
// 3. 플레이어 턴 종료 상태
public class PlayerTurnEndState : TurnStateBase
{
    public PlayerTurnEndState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = false;
        //ctx.turnStateText.text = "자신 턴 종료";
        Debug.Log("플레이어 턴 종료, 결과 업데이트(임시)");
        // 결과 업데이트 후 바로 AI 턴으로
        machine.ChangeState(new AiTurnState(ctx, machine));
    }
}
// 4. AI 턴 상태
public class AiTurnState : TurnStateBase
{
    public AiTurnState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        //ctx.turnStateText.text = "AI 턴 진행 중";
        Debug.Log("AI 행동 결정");
        // TODO: 여기서 AI 카드 선택 로직(지금은 더미)
        Debug.Log("AI 행동 플레이");
        Debug.Log("행동 결과 업데이트");
        Debug.Log("AI 턴 종료");
        // AI는 즉시 처리 후 다시 플레이어 턴 시작
        machine.ChangeState(new PlayerTurnStartState(ctx, machine));
    }
}