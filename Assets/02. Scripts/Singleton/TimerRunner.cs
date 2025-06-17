using UnityEngine;
using System.Collections.Generic;


public class TimerRunner : Singleton<TimerRunner>
{
    private List<Timer> timerList = new List<Timer>();

    public void Register(Timer timer)
    {
        timerList.Add(timer);
    }

    void Update()
    {
        for(int i = timerList.Count - 1; i >= 0; i--)
        {
            timerList[i].Update(Time.deltaTime);
            if (timerList[i].IsRunning == false)
            {
                timerList.RemoveAt(i);
            }
        }
    }
}
