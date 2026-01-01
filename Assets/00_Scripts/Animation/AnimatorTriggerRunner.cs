using System.Collections;
using UnityEngine;

// ==================================================================
// 목적 : 카드 행동 타입에 따라 Animator Trigger를 실행하고 Idle 복귀까지 대기
// 생성 일자 : 25/12/31
// 최근 수정 일자 : 25/12/31
// ==================================================================

public enum CardActionType { Attack, Defense, Heal }

public sealed class AnimatorTriggerRunner : MonoBehaviour
{   
    // Animator 컴포넌트 참조
    [SerializeField] private Animator animator;
    [SerializeField] private string idleStateName = "Idle";


    // 애니매이션 이름 해시 캐싱
    private int _idleHash;
    private readonly int _attack = Animator.StringToHash("Attack");
    private readonly int _defense = Animator.StringToHash("Defense");
    private readonly int _heal = Animator.StringToHash("Heal");

    private void Awake()
    {
        _idleHash = Animator.StringToHash(idleStateName);
    }

    /// <summary>
    /// 지정한 카드 행동 타입에 해당하는 애니메이션 트리거를 실행하고
    /// 애니메이션이 Idle 상태로 복귀할 때까지 대기하는 코루틴
    public IEnumerator PlayAndWaitIdle(CardActionType type, int layer = 0)
    {
        if (animator == null)
            yield break;

        // 트리거 경합 방지
        animator.ResetTrigger(_attack);
        animator.ResetTrigger(_defense);
        animator.ResetTrigger(_heal);

        switch (type)
        {
            case CardActionType.Attack:  animator.SetTrigger(_attack); break;
            case CardActionType.Defense: animator.SetTrigger(_defense); break;
            case CardActionType.Heal:    animator.SetTrigger(_heal); break;
        }

        // 1) Idle을 벗어날 때까지 대기
        yield return new WaitUntil(() =>
        {
            if (animator.IsInTransition(layer)) return false;
            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash != _idleHash;
        });

        // 2) 다시 Idle로 돌아올 때까지 대기
        yield return new WaitUntil(() =>
        {
            if (animator.IsInTransition(layer)) return false;
            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == _idleHash;
        });
    }
}
