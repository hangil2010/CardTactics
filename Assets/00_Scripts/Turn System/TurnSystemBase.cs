using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : 턴 진행에 필요한 컨텍스트(TurnContext)와 턴 상태 기초 클래스, 상태 머신의 기반 구조를 제공
// 생성 일자 : 12/08
// 최근 수정 일자 : 12/08
// ==================================================================

public class TurnContext
{
    public TMP_Text turnStateText;
    public Button turnEndButton;
}

public abstract class TurnStateBase
{
    protected TurnContext ctx;
    protected TurnStateMachine machine;

    // 생성자
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

// 턴 상태 머신 클래스
public class TurnStateMachine
{
    public TurnStateBase CurrentState { get; private set; }

    // 현재 턴 상태 변경
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

    // 턴 종료 버튼이 눌렸을 때 호출되는 메서드
    public void OnTurnEndButtonPressed()
    {
        CurrentState?.OnTurnEndButtonPressed();
    }
}


