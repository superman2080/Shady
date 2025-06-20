using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour
{
    public List<TutorialState> tutorialStates;
    private Queue<TutorialState> actionQueue = new Queue<TutorialState>();
    public TutorialState curTutorialState { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var item in tutorialStates)
        {
            actionQueue.Enqueue(item);
        }
        curTutorialState = actionQueue.Dequeue();
        curTutorialState.Enter(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (curTutorialState != null)
            curTutorialState.Execute(this);
    }

    public void SetNextTutorial()
    {
        if(curTutorialState != null)
        {
            curTutorialState.Exit(this);
        }
        if(actionQueue.TryDequeue(out TutorialState state))
        {
            curTutorialState = state;
            curTutorialState.Enter(this);
        }
        else
        {
            curTutorialState = null;
        }
    }
}
