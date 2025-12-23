using UnityEngine;

// ==================================================================
// 목적 : 게임 시작 시 카드 매니저에서 공격/방어 카드를 불러와 Choice Card UI에 바인딩
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/23
// ==================================================================
// [25/12/23] 수정 : 언어 통일성을 위해 이름을 ChoiceCardInitializer -> ActionCardInitializer 로 변경
public class ActionCardInitializer : MonoBehaviour
{
    [Header("선택 카드 UI")]
    [SerializeField] private ActionCardView attackActionCardView;
    [SerializeField] private ActionCardView defenseActionCardView;
    [SerializeField] private ActionCardView healActionCardView;

    [Header("행동 카드 데이터 매니저")]
    // [25/12/23] 수정 : ActionCardDataManager 인스턴스 참조용 필드 추가
    private ActionCardDataManager actionCardDataManager;

    // [25/12/23] 수정 : 카드 매니저의 카드 로드 완료 이벤트 구독 및 바인딩 처리
    private void OnEnable()
    {
        actionCardDataManager = ActionCardDataManager.Instance;

        if (actionCardDataManager == null)
        {
            Debug.LogError("[ActionCardInitializer] ActionCardDataManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        actionCardDataManager.OnCardsLoaded += BindCards;

        // 로드가 끝난 상태면 즉시 바인딩
        if (actionCardDataManager.IsReady)
            BindCards();
    }

    private void OnDisable()
    {
        if (ActionCardDataManager.Instance != null)
            ActionCardDataManager.Instance.OnCardsLoaded -= BindCards;
    }

    /// <summary>
    /// 카드 매니저에서 카드를 불러와 UI에 바인딩한다.
    /// </summary>
    private void BindCards()
    {
        var attackCard = actionCardDataManager.GetFirstCard(ActionCardData.ActionType.Attack);
        var defenseCard = actionCardDataManager.GetFirstCard(ActionCardData.ActionType.Defense);
        var healCard = actionCardDataManager.GetFirstCard(ActionCardData.ActionType.Heal);

        attackActionCardView?.SetData(attackCard);
        defenseActionCardView?.SetData(defenseCard);
        healActionCardView?.SetData(healCard);

        Debug.Log("[ActionCardInitializer] Action cards bound to UI.");
    }
}
