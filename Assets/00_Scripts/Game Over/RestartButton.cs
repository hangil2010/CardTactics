using UnityEngine;
using UnityEngine.SceneManagement;

// ==================================================================
// 목적 : 게임 재시작 버튼 클릭 시 현재 Scene 재로딩
// 생성 일자 : 26/01/02
// 최근 수정 일자 : 26/01/02
// ==================================================================

public class RestartButton : MonoBehaviour
{
    /// <summary>
    /// 현재 Scene 재시작
    /// </summary>
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
