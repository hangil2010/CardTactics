using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 플레이어 턴 시작/행동 결정/종료 및 AI 턴 진행 흐름을 관리하는 턴 상태 클래스 모음
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/16
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

        // [25/12/15] 수정: 현재는 가장 단순한 더미 AI - 공격 카드 1장을 3회 사용하도록 구성
        var mgr = ActionCardDataManager.Instance;
        ActionCardData aiDefaultAttack = null;

        if (mgr != null)
        {
            aiDefaultAttack = mgr.GetFirstCard(ActionCardData.ActionType.Attack);
        }

        ctx.aiPlannedCards[0] = aiDefaultAttack;
        ctx.aiPlannedCards[1] = aiDefaultAttack;
        ctx.aiPlannedCards[2] = aiDefaultAttack;

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

    /// <summary>
    /// 전투 사이클 실행, Player→Resolve→AI→Resolve를 최대 3회 실행
    /// </summary>
    // [25/12/15] 수정: CharactorData 기반 전투 사이클 적용
    private void ExecuteLoop()
    {
        var playerCards = ctx.selectedAreaManager != null ? ctx.selectedAreaManager.SelectedCards : null;

        for (int i = 0; i < 3; i++)
        {
            // ---------- Player Action ----------
            var pCard = (playerCards != null && i < playerCards.Count) ? playerCards[i] : null;
            Debug.Log($"[Cycle {i + 1}] Player 행동: {(pCard != null ? pCard.CardName : "None")}");

            if (pCard != null)
                ActionCardExecutor.Execute(pCard, ctx.playerCharactor, ctx.enemyCharactor);

            Debug.Log($"[Cycle {i + 1}] Resolve(Player) => " +
                    $"P_HP:{ctx.playerCharactor.GetHealth()}, P_Guard:{ctx.playerCharactor.GetIsGuarding()} / " +
                    $"E_HP:{ctx.enemyCharactor.GetHealth()}, E_Guard:{ctx.enemyCharactor.GetIsGuarding()}");

            if (IsBattleEnded())
            {
                machine.ChangeState(new BattleEndState(ctx, machine));
                return;
            }

            // ---------- AI Action ----------
            var aCard = (ctx.aiPlannedCards != null && i < ctx.aiPlannedCards.Length) ? ctx.aiPlannedCards[i] : null;
            Debug.Log($"[Cycle {i + 1}] AI 행동: {(aCard != null ? aCard.CardName : "None")}");

            if (aCard != null)
                ActionCardExecutor.Execute(aCard, ctx.enemyCharactor, ctx.playerCharactor);

            Debug.Log($"[Cycle {i + 1}] Resolve(AI) => " +
                    $"P_HP:{ctx.playerCharactor.GetHealth()}, P_Guard:{ctx.playerCharactor.GetIsGuarding()} / " +
                    $"E_HP:{ctx.enemyCharactor.GetHealth()}, E_Guard:{ctx.enemyCharactor.GetIsGuarding()}");

            if (IsBattleEnded())
            {
                machine.ChangeState(new BattleEndState(ctx, machine));
                return;
            }
        }

        Debug.Log("전투 사이클 종료");
        machine.ChangeState(new AllCycleEndState(ctx, machine));
    }

    private bool IsBattleEnded()
    {
        return ctx.playerCharactor.GetHealth() <= 0 || ctx.enemyCharactor.GetHealth() <= 0;
    }
}
#endregion

#region BattleEndState
/// <summary>
/// 전투 종료 상태(최소 구현: 로그 출력 + 버튼 비활성).
/// </summary>
public class BattleEndState : TurnStateBase
{
    public BattleEndState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        ctx.turnEndButton.interactable = false;

        // [25/12/15] 수정: CharactorData 기반 결과 판정
        int pHp = ctx.playerCharactor.GetHealth();
        int eHp = ctx.enemyCharactor.GetHealth();

        // [25/12/16] 추가 : 캐릭터 UI 업데이트를 여기에서 수행
        ctx.playerCharactorUI.UpdateHealthUI();
        ctx.enemyCharactorUI.UpdateHealthUI();

        string result =
            pHp <= 0 && eHp <= 0 ? "무승부" :
            pHp <= 0 ? "패배" :
            "승리";

        Debug.Log($"전투 종료: {result}");
        // TODO: 결과 UI 표시 / 재시작 / 다음 씬 등

    }
}
#endregion

#region AllCycleEndState
/// <summary>
/// 모든 사이클 완료 상태. 사이클 완료 후 플레이어 턴 시작으로 복귀한다.
/// </summary>
// [25/12/16] 수정 : AiTurnEndState -> AllCycleEndState 로 이름 변경
// AI의 턴 종료 상태를 굳이 하나의 사이클로 하기보다는, 모든 사이클이 종료됬을때로 상태를 변경
public class AllCycleEndState : TurnStateBase
{
    public AllCycleEndState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        Debug.Log("모든 사이클 완료");
        Debug.Log("다음 턴 : 플레이어 턴 시작");
        machine.ChangeState(new PlayerTurnStartState(ctx, machine));
    }
}
#endregion