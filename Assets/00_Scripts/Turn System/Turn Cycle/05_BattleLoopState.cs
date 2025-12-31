using System.Collections;
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
        //ExecuteLoop();
        // [25/12/31] 수정: Animation Trigger 기반 코루틴으로 변경
        ctx.coroutineRunner.StartCoroutine(ExecuteLoopCo());
    }

    /// <summary>
    /// 전투 사이클 실행 코루틴, Player→Resolve→AI→Resolve를 최대 3회 실행
    /// </summary>
    // [25/12/31] 추가: Animation Trigger 기반 코루틴으로 변경
    private IEnumerator ExecuteLoopCo()
    {
        var playerCards = ctx.selectedAreaManager != null ? ctx.selectedAreaManager.SelectedCards : null;
        
        // [25/12/31] 추가: 플레이어 및 AI 카드 유효성 검사, 기존에 매번 체크하던 것을 한번에 처리
        if(playerCards == null && (ctx.aiPlannedCards == null || ctx.aiPlannedCards.Length == 0))
        {
            Debug.LogWarning("전투 사이클 실행 불가: 플레이어 및 AI 카드 없음");
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            // cycle 플래그 초기화
            ctx.playerUsedHealThisCycle = false;
            ctx.enemyUsedHealThisCycle = false;
            ctx.playerUsedAttackThisCycle = false;
            ctx.enemyUsedAttackThisCycle = false;

            // 25/12/31 추가: 전투 사이클 텍스트 업데이트
            ctx.battleCycleText.text = $"Battle Cycle {i + 1} / 3";

            var pCard = (i < playerCards.Count) ? playerCards[i] : null;
            var aCard = (i < ctx.aiPlannedCards.Length) ? ctx.aiPlannedCards[i] : null;

            Debug.Log($"[Cycle {i + 1}] Player 행동: {pCard.CardName}");
            Debug.Log($"[Cycle {i + 1}] AI 행동: {aCard.CardName}");

            // 1) 애니메이션 재생 및 대기
            yield return ctx.playerAnim.PlayAndWaitIdle(ToAnimType(pCard.Type));
            yield return ctx.enemyAnim.PlayAndWaitIdle(ToAnimType(aCard.Type));

            // 2) 기록 저장
            if (ctx.playRecord != null)
            {
                ctx.playRecord.RecordPlayer(i, pCard.Type);
                ctx.playRecord.RecordEnemy(i, aCard.Type);
            }

            // 3) 먼저 방어/회복 같은 상태 세팅을 먼저 처리
            if (pCard.EffectData.Type != ActionCardEffectData.EffectType.Attack)
                ActionCardExecutor.Execute(pCard, ctx.playerCharactor, ctx.enemyCharactor, ctx, isPlayer: true);

            if (aCard.EffectData.Type != ActionCardEffectData.EffectType.Attack)
                ActionCardExecutor.Execute(aCard, ctx.enemyCharactor, ctx.playerCharactor, ctx, isPlayer: false);

            // 4) 그 다음 공격을 처리
            if (pCard.EffectData.Type == ActionCardEffectData.EffectType.Attack)
                ActionCardExecutor.Execute(pCard, ctx.playerCharactor, ctx.enemyCharactor, ctx, isPlayer: true);

            if (aCard.EffectData.Type == ActionCardEffectData.EffectType.Attack)
                ActionCardExecutor.Execute(aCard, ctx.enemyCharactor, ctx.playerCharactor, ctx, isPlayer: false);

            // 5) 사이클 종료 처리(Heal/UI/종료)
            ApplyHealAtEndOfCycle();
            ctx.playerCharactorUI.UpdateHealthUI();
            ctx.enemyCharactorUI.UpdateHealthUI();

            if (IsBattleEnded())
            {
                machine.ChangeState(new BattleEndState(ctx, machine));
                yield break;
            }
        }

        Debug.Log("전투 사이클 종료");
        // 25/12/31 추가: 전투 사이클 텍스트 초기화
        ctx.battleCycleText.text = $"Battle Cycle 1 / 3";
        machine.ChangeState(new AllCycleEndState(ctx, machine));
    }

    private CardActionType ToAnimType(ActionCardData.ActionType type)
    {
        return type switch
        {
            ActionCardData.ActionType.Attack => CardActionType.Attack,
            ActionCardData.ActionType.Defense => CardActionType.Defense,
            ActionCardData.ActionType.Heal => CardActionType.Heal,
            _ => CardActionType.Attack
        };
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
        int delta = opponentUsedAttack ? -1 : +2;
        unit.SetHealth(Mathf.Max(0, unit.GetHealth() + delta));
        Debug.Log($"[Heal] {unit.name} delta={delta}");
    }
}