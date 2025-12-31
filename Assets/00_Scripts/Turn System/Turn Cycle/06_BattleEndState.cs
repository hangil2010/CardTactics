using UnityEngine;

// ==================================================================
// 목적 : 전투 사이 종료 턴의 진행 흐름을 관리하는 클래스
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/31
// ==================================================================

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