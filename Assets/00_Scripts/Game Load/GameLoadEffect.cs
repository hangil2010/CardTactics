using System.Collections;
using UnityEngine;

// ==================================================================
// 목적 : Addressables 로딩 완료 시 로딩 화면 FadeOut 연출 후 게임 화면으로 전환
// 생성 일자 : 26/01/02
// 최근 수정 일자 : 26/01/02
// ==================================================================
public class GameLoadEffect : MonoBehaviour
{
    [Header("Loading UI Root")]
    [SerializeField] private GameObject loadingScreenRoot;

    [Header("Animator")]
    [SerializeField] private Animator loadingAnimator;

    [Header("After FadeOut")]
    [SerializeField] private bool disableRootAfterFadeOut = true;
    [SerializeField] private bool destroyRootAfterFadeOut = false;

    private bool _played;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        // 시작 시 로딩 화면은 보이게, 애니메이터는 꺼둠(원하는 방식)
        if (loadingScreenRoot != null)
            loadingScreenRoot.SetActive(true);

        if (loadingAnimator != null)
            loadingAnimator.enabled = false;
    }

    private void OnEnable()
    {
        
        if (ActionCardDataManager.Instance != null)
            ActionCardDataManager.Instance.OnCardsLoaded += HandleCardsLoaded;

        TryPlayIfAlreadyReady();
    }

    private void OnDisable()
    {
        if (ActionCardDataManager.Instance != null)
            ActionCardDataManager.Instance.OnCardsLoaded -= HandleCardsLoaded;
    }

    private void TryPlayIfAlreadyReady()
    {
        var mgr = ActionCardDataManager.Instance;
        if (mgr != null && mgr.IsReady)
            HandleCardsLoaded();
    }

    private void HandleCardsLoaded()
    {
        if (_played) return;
        _played = true;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(PlayFadeOutAndClose());
    }

    private IEnumerator PlayFadeOutAndClose()
    {
        if (loadingAnimator == null)
        {
            Debug.LogWarning("[GameLoadEffect] loadingAnimator가 null 입니다. 로딩 화면을 즉시 종료합니다.");
            CloseLoadingScreen();
            yield break;
        }

        Debug.Log("[GameLoadEffect] 로딩 이펙트 시작");
        
        // 1) Animator 켜기
        loadingAnimator.enabled = true;

        // 2) 상태 정보 갱신을 위해 1프레임 대기
        yield return null;

        // 3) 현재 state 길이만큼 대기
        var stateInfo = loadingAnimator.GetCurrentAnimatorStateInfo(0);

        float wait = stateInfo.length > 0.01f ? stateInfo.length : 2f;
        yield return new WaitForSeconds(wait);

        CloseLoadingScreen();
    }

    private void CloseLoadingScreen()
    {
        if (loadingScreenRoot == null) return;

        if (destroyRootAfterFadeOut)
        {
            Destroy(loadingScreenRoot);
            return;
        }

        if (disableRootAfterFadeOut)
            loadingScreenRoot.SetActive(false);
    }
}
