using System;
using UnityEngine;

public class Timer
{
    private float duration;
    private float timeLeft;
    private bool isRunning = false;

    private Action onComplete;
    private Action onUpdate;

    public bool IsRunning => isRunning;
    public float TimeLeft => timeLeft;

    public Timer(float duration, Action onComplete = null, Action onUpdate = null, bool autoStart = true)
    {
        this.duration = duration;
        this.timeLeft = duration;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        Start();
        if (autoStart)
            TimerRunner.Instance.Register(this);
    }

    public void Start()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Reset(Action onReset = null)
    {
        timeLeft = duration;
        onReset?.Invoke();
        Start();
    }

    public void Update(float deltaTime)
    {
        if (!isRunning) return;

        if (timeLeft <= 0f)
        {
            isRunning = false;
            onComplete?.Invoke();
        }
        else
        {
            timeLeft = Mathf.Clamp(timeLeft - deltaTime, 0, duration);
            onUpdate?.Invoke();
        }
    }
}
