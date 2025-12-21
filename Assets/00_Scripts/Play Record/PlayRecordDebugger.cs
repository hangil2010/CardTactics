using UnityEngine;

// ==================================================================
// 목적 : PlayRecord의 행동 기록을 키 입력으로 콘솔에 출력한다
// 생성 일자 : 25/12/21
// 최근 수정 일자 : 25/12/21
// ==================================================================

public class PlayRecordDebugInput : MonoBehaviour
{
    [SerializeField] private PlayRecord playRecord;

    private void Update()
    {
        if (playRecord == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(playRecord.BuildLogForPlayer());
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log(playRecord.BuildLogForEnemy());
        }
    }
}
