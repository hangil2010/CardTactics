using UnityEngine;

// ==================================================================
// 목적 : 모든 사이클 완료 시 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

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
        Debug.Log("===============모든 사이클 완료===============");
        ctx.selectedAreaManager?.ResetSelection();
        // [25/12/30] 추가 : 턴 카운트 증가 및 UI 갱신
        ctx.currentTurnCount++;
        ctx.turnCountText.text = $"Turn {ctx.currentTurnCount}";
        Debug.Log("다음 턴 : 플레이어 턴 시작");
        machine.ChangeState(new PlayerTurnStartState(ctx, machine));
    }
}