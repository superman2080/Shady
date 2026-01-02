using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CoroutineRunner : Singleton<CoroutineRunner>
{
    private Dictionary<int, Coroutine> _runningCoroutines = new Dictionary<int, Coroutine>();
    private int _nextCoroutineId = 0;

    public int StartManagedCoroutine(IEnumerator coroutine)
    {
        if (coroutine == null)
        {
            return -1;
        }

        int coroutineId = _nextCoroutineId++;
        Coroutine runningCoroutine = StartCoroutine(ExecuteCoroutine(coroutine, coroutineId));
        _runningCoroutines[coroutineId] = runningCoroutine;

        return coroutineId;
    }

    /// <summary>
    /// 지정된 ID의 코루틴을 중지합니다
    /// </summary>
    /// <param name="coroutineId">중지할 코루틴의 ID</param>
    public void StopManagedCoroutine(int coroutineId)
    {
        if (_runningCoroutines.TryGetValue(coroutineId, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            _runningCoroutines.Remove(coroutineId);
        }
    }

    /// <summary>
    /// 모든 관리되는 코루틴을 중지합니다
    /// </summary>
    public void StopAllManagedCoroutines()
    {
        foreach (var pair in _runningCoroutines)
        {
            if (pair.Value != null)
            {
                StopCoroutine(pair.Value);
            }
        }
        _runningCoroutines.Clear();
    }

    /// <summary>
    /// 지정된 ID의 코루틴이 실행 중인지 확인합니다
    /// </summary>
    /// <param name="coroutineId">확인할 코루틴의 ID</param>
    /// <returns>실행 중이면 true, 아니면 false</returns>
    public bool IsCoroutineRunning(int coroutineId)
    {
        return _runningCoroutines.ContainsKey(coroutineId) && _runningCoroutines[coroutineId] != null;
    }

    /// <summary>
    /// 현재 실행 중인 코루틴의 개수를 반환합니다
    /// </summary>
    public int RunningCoroutineCount => _runningCoroutines.Count;

    /// <summary>
    /// 코루틴을 실행하고 완료 시 자동으로 정리하는 래퍼
    /// </summary>
    private IEnumerator ExecuteCoroutine(IEnumerator coroutine, int coroutineId)
    {
        yield return coroutine;

        // 코루틴이 정상 완료되면 딕셔너리에서 제거
        _runningCoroutines.Remove(coroutineId);
    }

    /// <summary>
    /// 간편하게 코루틴을 시작할 수 있는 정적 메서드
    /// </summary>
    /// <param name="coroutine">실행할 코루틴</param>
    /// <returns>코루틴 관리를 위한 고유 ID</returns>
    public static int Start(IEnumerator coroutine)
    {
        return Instance.StartManagedCoroutine(coroutine);
    }

    /// <summary>
    /// 간편하게 코루틴을 중지할 수 있는 정적 메서드
    /// </summary>
    /// <param name="coroutineId">중지할 코루틴의 ID</param>
    public static void Stop(int coroutineId)
    {
        Instance.StopManagedCoroutine(coroutineId);
    }

    /// <summary>
    /// 간편하게 모든 코루틴을 중지할 수 있는 정적 메서드
    /// </summary>
    public static void StopAll()
    {
        Instance.StopAllManagedCoroutines();
    }
}
