using System.Text;
using UnityEngine;

// ==================================================================
// 목적 : 전투 중 Player/Enemy의 행동 기록을 슬롯(1~3) 단위로 저장한다
// 생성 일자 : 25/12/21
// 최근 수정 일자 : 25/12/22
// ==================================================================

public class PlayRecord : MonoBehaviour
{
    /// <summary> 행위자 구분 </summary>
    public enum Actor
    {
        Player = 0,
        Enemy = 1
    }

    [Header("History Settings")]
    [SerializeField, Range(5, 50)] private int windowSize = 15;

    // [actor][slot][index]
    private ActionCardData.ActionType[,,] _history;
    private int[,] _writeIndex; // [actor][slot]
    private int[,] _count;      // [actor][slot] 현재 저장된 개수

    // [actor][slot][typeIndex] 카운트
    private int[,,] _typeCounts;

    private void Awake()
    {
        _history = new ActionCardData.ActionType[2, 3, windowSize];
        _writeIndex = new int[2, 3];
        _count = new int[2, 3];
        _typeCounts = new int[2, 3, 3];
    }

    public void RecordPlayer(int slotIndex, ActionCardData.ActionType type) => Record(Actor.Player, slotIndex, type);
    public void RecordEnemy(int slotIndex, ActionCardData.ActionType type) => Record(Actor.Enemy, slotIndex, type);

    /// <summary>
    /// 행위자와 슬롯 인덱스에 해당하는 기록을 저장한다.
    /// </summary>
    /// <param name="actor">행위자 (Player/Enemy)</param>
    /// <param name="slotIndex">슬롯 인덱스 (0~2)</param>
    /// <param name="type">행동 카드 타입</param>
    public void Record(Actor actor, int slotIndex, ActionCardData.ActionType type)
    {
        if (slotIndex < 0 || slotIndex >= 3) return;

        int a = (int)actor;
        int t = ToTypeIndex(type);
        if (t < 0) return;

        // 꽉 찼으면 덮어쓰기 전 기존 값 제거
        if (_count[a, slotIndex] >= windowSize)
        {
            int removeIndex = _writeIndex[a, slotIndex];
            var oldType = _history[a, slotIndex, removeIndex];
            int oldT = ToTypeIndex(oldType);
            if (oldT >= 0) _typeCounts[a, slotIndex, oldT]--;
        }
        else
        {
            _count[a, slotIndex]++;
        }

        // 기록
        int w = _writeIndex[a, slotIndex];
        _history[a, slotIndex, w] = type;
        _typeCounts[a, slotIndex, t]++;

        _writeIndex[a, slotIndex] = (w + 1) % windowSize;
    }
    /// <summary>
    /// 슬롯별 경향성 비율 추출
    /// </summary>
    /// <param name="actor">행위자 (Player/Enemy)</param>
    /// <param name="slotIndex">슬롯 인덱스 (0~2)</param>
    /// <param name="attackRate">공격 비율 출력</param>
    /// <param name="defenseRate">방어 비율 출력</param>
    /// <param name="healRate">회복 비율 출력</param>
    // Stage 4 AI에서 쓰기 위한 “슬롯별 경향성 비율” (0~1)
    public void GetTendency(Actor actor, int slotIndex, out float attackRate, out float defenseRate, out float healRate)
    {
        attackRate = defenseRate = healRate = 0f;
        if (slotIndex < 0 || slotIndex >= 3) return;

        int a = (int)actor;
        int n = _count[a, slotIndex];
        if (n <= 0) return;

        attackRate = _typeCounts[a, slotIndex, 0] / (float)n;
        defenseRate = _typeCounts[a, slotIndex, 1] / (float)n;
        healRate = _typeCounts[a, slotIndex, 2] / (float)n;
    }

    /// <summary>
    /// ActionType을 인덱스로 변환한다.
    /// </summary>
    /// <param name="type">행동 카드 타입</param>
    private int ToTypeIndex(ActionCardData.ActionType type)
    {
        // 프로젝트의 ActionType이 Attack/Defense/Heal 이라는 전제
        return type switch
        {
            ActionCardData.ActionType.Attack => 0,
            ActionCardData.ActionType.Defense => 1,
            ActionCardData.ActionType.Heal => 2,
            _ => -1
        };
    }

    /// <summary>
    /// 해당 슬롯에 최소 샘플 개수가 있는지 확인한다.
    /// </summary>
    public bool HasEnoughSamples(Actor actor, int slotIndex, int minSamples)
    {
        int a = (int)actor;
        if (slotIndex < 0 || slotIndex >= 3) return false;
        return _count[a, slotIndex] >= minSamples;
    }
    /// <summary>
    /// 해당 슬롯의 샘플 개수를 반환한다.
    /// </summary>
    public int GetSampleCount(Actor actor, int slotIndex)
    {
        int a = (int)actor;
        if (slotIndex < 0 || slotIndex >= 3) return 0;
        return _count[a, slotIndex];
    }

    #region Log Building
    public string BuildLogForPlayer()
    {
        return BuildLog(Actor.Player);
    }

    public string BuildLogForEnemy()
    {
        return BuildLog(Actor.Enemy);
    }
    /// <summary>
    /// 행위자별 로그 문자열 생성, StringBuilder 사용
    /// </summary>
    private string BuildLog(Actor actor)
    {
        var sb = new StringBuilder(512);

        sb.AppendLine("====================================");
        sb.AppendLine($"[PlayRecord] {(actor == Actor.Player ? "PLAYER" : "ENEMY")} ACTION LOG");
        sb.AppendLine("====================================");

        int a = (int)actor;

        for (int slot = 0; slot < 3; slot++)
        {
            sb.AppendLine($"-- Slot {slot + 1} --");

            int count = _count[a, slot];
            if (count == 0)
            {
                sb.AppendLine("  (no records)");
                continue;
            }

            // 최근 기록 출력 (오래된 → 최신)
            sb.Append("  Recent : ");
            for (int i = 0; i < count; i++)
            {
                int index = (_writeIndex[a, slot] - count + i + windowSize) % windowSize;
                sb.Append(_history[a, slot, index]);
                if (i < count - 1) sb.Append(" -> ");
            }
            sb.AppendLine();

            // 비율 출력
            float atk = _typeCounts[a, slot, 0] / (float)count;
            float def = _typeCounts[a, slot, 1] / (float)count;
            float heal = _typeCounts[a, slot, 2] / (float)count;

            sb.AppendLine($"  Rate   : Attack {atk:P0}, Defense {def:P0}, Heal {heal:P0}");
        }

        sb.AppendLine("====================================");
        return sb.ToString();
    }
    #endregion
}
