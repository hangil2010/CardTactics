using System;
using UnityEngine;

// ==================================================================
// 목적 : Resources , Addressables 로드 성능 측정용 벤치마크 로거
// 생성 일자 : 25/12/23
// 최근 수정 일자 : 25/12/23
// ==================================================================

public static class BenchmarkLogger
{
    // Stopwatch 인스턴스 재사용을 위해 static으로 보관
    private static readonly System.Diagnostics.Stopwatch Sw = new System.Diagnostics.Stopwatch();

    /// <summary>
    /// 벤치마크 시작 로그 기록
    /// </summary>
    public static void Begin(string tag)
    {
        Sw.Reset();
        Sw.Start();
        Debug.Log($"[Benchmark][{tag}] BEGIN time={Time.realtimeSinceStartup:F3}");
    }

    /// <summary>
    /// 벤치마크 종료 로그 기록
    /// </summary>
    public static void End(string tag)
    {
        Sw.Stop();
        var ms = Sw.Elapsed.TotalMilliseconds;
        Debug.Log($"[Benchmark][{tag}] END ms={ms:F2} time={Time.realtimeSinceStartup:F3}");
    }
}
