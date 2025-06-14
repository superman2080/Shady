using System;
using UnityEngine;

public class Timer
{
    private float duration;

    private Action onComplete;

    public bool IsRunning => isRunning;
    public float TimeLeft => timeLeft;
    private float timeLeft;
    private bool isRunning = false;

    public Timer(float duration, Action onComplete)
    {
        this.duration = duration;
        this.timeLeft = duration;
        this.onComplete = onComplete;
        Start();
    }

    public void Start()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Reset(Action onReset)
    {
        timeLeft = duration;
        isRunning = true;
        onReset?.Invoke();
    }

    public void Update(float deltaTime)
    {
        if (!isRunning) return;

        timeLeft -= deltaTime;
        if (timeLeft <= 0f)
        {
            isRunning = false;
            onComplete?.Invoke();
        }
    }

}
