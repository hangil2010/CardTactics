using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : 턴 진행에 필요한 컨텍스트(TurnContext)와 턴 상태 기초 클래스, 상태 머신의 기반 구조를 제공
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 25/12/16
// ==================================================================

/// <summary>
/// 턴 상태 머신이 사용하는 데이터 및 UI 참조를 보유한 컨텍스트.
/// Domain 로직과 Presentation(UI)을 연결하는 역할을 수행한다.
/// </summary>
public class TurnContext
{
    /// <summary>현재 턴 상태를 표시하는 텍스트 UI. </summary>
    public TMP_Text turnStateText;

    /// <summary>플레이어 턴 종료 버튼.</summary>
    public Button turnEndButton;

    // 25/12/15 추가
    /// <summary> 선택된 카드(플레이어) 조회용 </summary>
    public SelectedAreaManager selectedAreaManager;

    // 25/12/15 추가
    /// <summary> AI가 이번 사이클에서 사용할 행동(최대 3개)을 저장 </summary>
    public ActionCardData[] aiPlannedCards;

    // 25/12/15 추가, CharactorData 참조 사용
    public CharactorData playerCharactor;
    public CharactorData enemyCharactor;

    // 25/12/16 추가 : CharactorController에서 UI 참조
    public CharactorController playerCharactorUI;
    public CharactorController enemyCharactorUI;
}

/// <summary>
/// 모든 턴 상태의 공통 기반 클래스.
/// 상태 진입, 종료, 업데이트(Tick), 버튼 입력 처리 메서드를 제공한다.
/// </summary>
public abstract class TurnStateBase
{
    // 상태에서 사용할 컨텍스트(UI, 데이터).
    protected TurnContext ctx;

    // 상태 간 전환을 담당하는 상태 머신.
    protected TurnStateMachine machine;

    // 기본 생성자
    protected TurnStateBase(TurnContext ctx, TurnStateMachine machine)
    {
        this.ctx = ctx;
        this.machine = machine;
    }

    // 상태 진입, 종료, 틱 메서드
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }

    // 버튼 눌렸을 때 상태별로 처리
    public virtual void OnTurnEndButtonPressed() { }
}

/// <summary>
/// 턴 상태들을 관리하고 전환을 제어하는 상태 머신.
/// CurrentState를 중심으로 Enter/Exit/Tick 흐름을 유지한다.
/// </summary>
public class TurnStateMachine
{
    // 현재 활성 상태
    public TurnStateBase CurrentState { get; private set; }

    /// <summary>
    /// 상태 전환을 수행하며 이전 상태 Exit → 새 상태 Enter 순으로 호출한다.
    /// </summary>
    public void ChangeState(TurnStateBase next)
    {
        CurrentState?.Exit();
        CurrentState = next;
        CurrentState.Enter();
    }

    // 매 프레임마다 호출되는 틱 메서드
    public void Tick()
    {
        CurrentState?.Tick();
    }

    /// <summary>
    /// UI 입력(턴 종료 버튼)을 현재 상태에 전달한다.
    /// </summary>
    public void OnTurnEndButtonPressed()
    {
        CurrentState?.OnTurnEndButtonPressed();
    }
}


