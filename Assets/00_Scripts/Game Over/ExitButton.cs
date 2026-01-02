using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==================================================================
// 목적 : 게임 종료 버튼 클릭 시 발동할 Events
// 생성 일자 : 25/12/08
// 최근 수정 일자 : 26/01/02
// ==================================================================

public class ExitButton : MonoBehaviour
{
    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
