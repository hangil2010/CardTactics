using UnityEngine;

// ==================================================================
// 목적 : 전투 진행 루프 턴의 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

/// <summary>
/// 전투 사이클을 수행하는 상태.
/// Player → Resolve → AI → Resolve 흐름을 최대 3회 반복한다.
/// </summary>
public class BattleLoopState : TurnStateBase
{

    public BattleLoopState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
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
            // Reset guarding state at the start of each cycle
            ctx.playerUsedHealThisCycle = false;
            ctx.enemyUsedHealThisCycle = false;
            ctx.playerUsedAttackThisCycle = false;
            ctx.enemyUsedAttackThisCycle = false;

            // ---------- Player Action ----------
            var pCard = (playerCards != null && i < playerCards.Count) ? playerCards[i] : null;
            Debug.Log($"[Cycle {i + 1}] Player 행동: {(pCard != null ? pCard.CardName : "None")}");

            // [25/12/21] 슬롯별 기록 저장 (Player)
            if (pCard != null && ctx.playRecord != null)
                ctx.playRecord.RecordPlayer(i, pCard.Type);

            if (pCard != null)
                ActionCardExecutor.Execute(pCard, ctx.playerCharactor, ctx.enemyCharactor,ctx ,isPlayer : true);

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

            // [25/12/21] 슬롯별 기록 저장 (Enemy)
            if (aCard != null && ctx.playRecord != null)
                ctx.playRecord.RecordEnemy(i, aCard.Type);

            if (aCard != null)
                ActionCardExecutor.Execute(aCard, ctx.enemyCharactor, ctx.playerCharactor,ctx ,isPlayer : false);

            Debug.Log($"[Cycle {i + 1}] Resolve(AI) => " +
                    $"P_HP:{ctx.playerCharactor.GetHealth()}, P_Guard:{ctx.playerCharactor.GetIsGuarding()} / " +
                    $"E_HP:{ctx.enemyCharactor.GetHealth()}, E_Guard:{ctx.enemyCharactor.GetIsGuarding()}");

            // [25/12/19] 추가 : 사이클 종료 시 회복 효과 적용
            ApplyHealAtEndOfCycle();

            // [25/12/16] 추가 : 캐릭터 UI 업데이트를 여기에서 수행
            // 전투 사이클 내에서 체력 변화가 있을 수 있으므로 매 사이클마다 UI를 갱신
            ctx.playerCharactorUI.UpdateHealthUI();
            ctx.enemyCharactorUI.UpdateHealthUI();

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

    private void ApplyHealAtEndOfCycle()
    {
        if (ctx.playerUsedHealThisCycle)
            ApplyHealDelta(ctx.playerCharactor, ctx.enemyUsedAttackThisCycle);

        if (ctx.enemyUsedHealThisCycle)
            ApplyHealDelta(ctx.enemyCharactor, ctx.playerUsedAttackThisCycle);
    }

    private void ApplyHealDelta(CharactorData unit, bool opponentUsedAttack)
    {
        int delta = opponentUsedAttack ? -2 : +2;
        unit.SetHealth(Mathf.Max(0, unit.GetHealth() + delta));
        Debug.Log($"[Heal] {unit.name} delta={delta}");
    }
}