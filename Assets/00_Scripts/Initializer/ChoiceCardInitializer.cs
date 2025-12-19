using UnityEngine;

// ==================================================================
// 목적 : 게임 시작 시 카드 매니저에서 공격/방어 카드를 불러와 Choice Card UI에 바인딩
// 생성 일자 : 25/12/10
// 최근 수정 일자 : 25/12/19
// ==================================================================

public class ChoiceCardInitializer : MonoBehaviour
{
    [Header("선택 카드 UI")]
    [SerializeField] private ActionCardView attackActionCardView;
    [SerializeField] private ActionCardView defenseActionCardView;
    [SerializeField] private ActionCardView healActionCardView;
    private void Start()
    {
        var manager = ActionCardDataManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[ChoiceCardInitializer] ActionCardDataManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 공격/방어 카드 중 첫 번째 카드를 가져와 바인딩
        var attackCard = manager.GetFirstCard(ActionCardData.ActionType.Attack);
        var defenseCard = manager.GetFirstCard(ActionCardData.ActionType.Defense);
        var healCard = manager.GetFirstCard(ActionCardData.ActionType.Heal);

        if (attackActionCardView != null)
        {
            attackActionCardView.SetData(attackCard);
        }

        if (defenseActionCardView != null)
        {
            defenseActionCardView.SetData(defenseCard);
        }

        if (healActionCardView != null)
        {
            healActionCardView.SetData(healCard);
        }
    }
}
