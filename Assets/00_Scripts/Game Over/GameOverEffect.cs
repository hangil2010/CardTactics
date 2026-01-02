using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ==================================================================
// 목적 : 전투 종료 시 Game Over Canvas 활성화 후 Fade In 연출 재생
// 생성 일자 : 26/01/02
// 최근 수정 일자 : 26/01/02
// ==================================================================
public class GameOverEffect : MonoBehaviour
{
    [Header("Game Over UI Root")]
    [SerializeField] private Canvas gameOverCanvasRoot;
    [SerializeField] private GraphicRaycaster raycaster;

    [Header("Animator")]
    [SerializeField] private Animator gameOverAnimator;

    [Header("Battle Result Text")]
    [SerializeField] private TMP_Text battleResultText;
    private bool _played;

    private void Awake()
    {
        // 시작 시 숨김(원하는 방식)
        if (gameOverCanvasRoot != null)
        {
            gameOverCanvasRoot.gameObject.SetActive(true);
            raycaster.enabled = false;
        }

        if (gameOverAnimator != null)
            gameOverAnimator.enabled = false;
    }

    /// <summary>
    /// 전투 종료 시 호출: Canvas 활성화 후 FadeIn 애니메이션 재생
    /// </summary>
    public void PlayGameOverFadeIn(BattleResult result)
    {
        if (_played) return;
        _played = true;

        battleResultText.text = GetResultText(result);

        if (gameOverCanvasRoot != null)
        {
            gameOverCanvasRoot.gameObject.SetActive(true);
            raycaster.enabled = true;
        }

        if (gameOverAnimator == null)
        {
            Debug.LogWarning("[GameOverEffect] gameOverAnimator가 null 입니다.");
            return;
        }

        gameOverAnimator.enabled = true;
    }

    private string GetResultText(BattleResult result)
    {
        switch (result)
        {
            case BattleResult.Win:  return "You Win!";
            case BattleResult.Lose: return "You Lose...";
            case BattleResult.Draw: return "It's a Draw...";
            default:                return string.Empty;
        }
    }
}
