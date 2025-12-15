using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 시작/행동 결정/종료 및 AI 턴 진행 흐름을 관리하는 턴 상태 클래스 모음
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/05
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
        Debug.Log("플레이어 턴 종료");

        // [25/12/15] 수정: 플레이어 턴 종료 후 AI 즉시 결정 → 전투 사이클로 진입
        machine.ChangeState(new AiDecideState(ctx, machine));
    }
}
#endregion

#region AiDecideState
/// <summary>
/// AI가 이번 사이클에서 사용할 행동(최대 3개)을 즉시 결정하는 상태.
/// 실제 AI 로직이 붙기 전까지는 더미 카드/더미 선택을 구성한다.
/// </summary>
public class AiDecideState : TurnStateBase
{
    public AiDecideState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        Debug.Log("AI 행동 결정 (즉시 결정)");

        // [25/12/15] 추가: AI 플랜 배열이 없으면 생성 (최대 3개)
        if (ctx.aiPlannedCards == null || ctx.aiPlannedCards.Length != 3)
        {
            ctx.aiPlannedCards = new ActionCardData[3];
        }

        // [25/12/15] 추가: 더미 결정 로직 (현재는 null로 두고 로그만 찍어도 됨)
        // TODO: 실제 AI 카드 선택 로직 연결
        ctx.aiPlannedCards[0] = null;
        ctx.aiPlannedCards[1] = null;
        ctx.aiPlannedCards[2] = null;

        machine.ChangeState(new BattleLoopState(ctx, machine));
    }
}
#endregion

#region BattleLoopState
/// <summary>
/// 전투 사이클을 수행하는 상태.
/// Player → Resolve → AI → Resolve 흐름을 최대 3회 반복한다.
/// </summary>
public class BattleLoopState : TurnStateBase
{
    private int _index;

    public BattleLoopState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        _index = 0;
        Debug.Log("전투 사이클 시작 (최대 3회)");

        // [25/12/15] 추가: Player→Resolve→AI→Resolve를 최대 3회 실행
        ExecuteLoop();
    }

    // [25/12/15] 추가: Player→Resolve→AI→Resolve를 최대 3회 실행
    /// <summary>
    /// 전투 사이클 실행, Player→Resolve→AI→Resolve를 최대 3회 실행
    /// </summary>
    private void ExecuteLoop()
    {
        var playerCards = ctx.selectedAreaManager != null ? ctx.selectedAreaManager.SelectedCards : null;

        for (_index = 0; _index < 3; _index++)
        {
            // Player Action
            var pCard = (playerCards != null && _index < playerCards.Count) ? playerCards[_index] : null;
            Debug.Log($"[Cycle {_index + 1}] Player 행동 플레이: {(pCard != null ? pCard.CardName : "None")}");

            Debug.Log($"[Cycle {_index + 1}] 행동 결과 업데이트 (Player)");
            // TODO: 여기서 플레이어 행동 적용 + HP/사망 체크

            // AI Action
            var aCard = (ctx.aiPlannedCards != null && _index < ctx.aiPlannedCards.Length) ? ctx.aiPlannedCards[_index] : null;
            Debug.Log($"[Cycle {_index + 1}] AI 행동 플레이: {(aCard != null ? aCard.CardName : "None")}");

            Debug.Log($"[Cycle {_index + 1}] 행동 결과 업데이트 (AI)");
            // TODO: 여기서 AI 행동 적용 + HP/사망 체크
        }

        Debug.Log("전투 사이클 종료");
        machine.ChangeState(new AiTurnEndState(ctx, machine));
    }
}
#endregion

#region AiTurnEndState
/// <summary>
/// AI 턴 종료 상태. 사이클 완료 후 플레이어 턴 시작으로 복귀한다.
/// </summary>
public class AiTurnEndState : TurnStateBase
{
    public AiTurnEndState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        Debug.Log("AI 턴 종료");
        machine.ChangeState(new PlayerTurnStartState(ctx, machine));
    }
}
#endregion