using UnityEngine;

// ==================================================================
// 목적 : AI의 행동 결정 턴의 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

/// <summary>
/// AI가 이번 사이클에서 사용할 행동(최대 3개)을 즉시 결정하는 상태.
/// 실제 AI 로직이 붙기 전까지는 더미 카드/더미 선택을 구성한다.
/// </summary>
public class AiDecideState : TurnStateBase
{
    // Stage4 트리는 고정 구조라 캐싱 권장(매 Enter마다 Build()하지 않음)
    private static readonly BehaviorTreeNode _stage4Root = Stage4BehaviorTreeBuilder.Build();

    public AiDecideState(TurnContext ctx, TurnStateMachine machine) : base(ctx, machine) { }

    public override void Enter()
    {
        // 1) Stage4 BT 실행 -> 슬롯별 가중치 배열 세팅
        _stage4Root.Tick(ctx);

        // 2) 디버그 로그 (Stage4 판단 결과 확인)
        LogStage4Decision(ctx);

        // 3) 세팅된 슬롯별 가중치로 3장 계획
        AiPlanner.Plan3(ctx);

        // 4) 첫 판단 규칙 등 카운터
        ctx.aiDecisionCount++;

        machine.ChangeState(new BattleLoopState(ctx, machine));
    }

    private void LogStage4Decision(TurnContext ctx)
    {
        // playRecord 없으면 Stage4 판단 자체가 불가능하니 최소 로그만
        if (ctx.playRecord == null)
        {
            Debug.Log("[AI][Stage4] playRecord is null -> fallback weights applied (Stage3).");
            Debug.Log($"[AI][Stage4] SlotWeights A/D/H = " +
                      $"S1({ctx.aiAttackWeightsBySlot[0]}/{ctx.aiDefenseWeightsBySlot[0]}/{ctx.aiHealWeightsBySlot[0]}), " +
                      $"S2({ctx.aiAttackWeightsBySlot[1]}/{ctx.aiDefenseWeightsBySlot[1]}/{ctx.aiHealWeightsBySlot[1]}), " +
                      $"S3({ctx.aiAttackWeightsBySlot[2]}/{ctx.aiDefenseWeightsBySlot[2]}/{ctx.aiHealWeightsBySlot[2]})");
            return;
        }

        // Player 슬롯별 경향성 + 샘플 수 + 최종 가중치 출력
        for (int slot = 0; slot < 3; slot++)
        {
            int n = ctx.playRecord.GetSampleCount(PlayRecord.Actor.Player, slot);
            ctx.playRecord.GetTendency(PlayRecord.Actor.Player, slot, out float atk, out float def, out float heal);

            string dominant = GetDominant(atk, def, heal, 0.6f); // threshold는 Builder와 동일
            Debug.Log(
                $"[AI][Stage4] Slot{slot + 1} PlayerSamples={n} " +
                $"Rates(A/D/H)={atk:P0}/{def:P0}/{heal:P0} Dominant={dominant} " +
                $"-> Weights(A/D/H)={ctx.aiAttackWeightsBySlot[slot]}/{ctx.aiDefenseWeightsBySlot[slot]}/{ctx.aiHealWeightsBySlot[slot]}"
            );
        }
    }

    private string GetDominant(float atk, float def, float heal, float threshold)
    {
        float max = atk;
        string dominant = "Attack";
        if (def > max) { max = def; dominant = "Defense"; }
        if (heal > max) { max = heal; dominant = "Heal"; }
        return (max >= threshold) ? dominant : "None";
    }
}