using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour
{
    public List<StateBase<TutorialController>> tutorialStates;
    private Queue<StateBase<TutorialController>> actionQueue = new Queue<StateBase<TutorialController>>();
    public StateBase<TutorialController> curTutorialState { get; private set; }


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
        if(actionQueue.TryDequeue(out StateBase<TutorialController> state))
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
