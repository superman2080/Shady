using System.Collections;
using UnityEngine;

[System.Serializable]
public class StateMachine<T> where T: MonoBehaviour
{
    public IState<T> State { get => state; }
    private IState<T> state;
    private T caster;
    private Coroutine delayCor;


    public StateMachine(T caster, IState<T> state)
    {
        this.caster = caster;
        this.state = state;
        this.state.Enter(caster);
    }

    public void ChangeState(IState<T> newState, float delayTime = 0.5f)
    {
        if (state == null || CompareState(state, newState) || delayCor != null) return;

        delayCor = caster.StartCoroutine(DelayChangeStateCor(delayTime, newState));
    }

    public void ChangeStateImmediately(IState<T> newState)
    {
        if (state == null || CompareState(state, newState)) return;

        state.Exit(caster);
        state = newState;
        state.Enter(caster);
    }

    public void Update()
    {
        if (state != null)
            state.Execute(caster);
    }

    private IEnumerator DelayChangeStateCor(float sec, IState<T> newState)
    {
        state.Exit(caster);
        state = null;
        yield return new WaitForSeconds(sec);
        delayCor = null;
        state = newState;
        state.Enter(caster);
    }

    public bool CompareState(IState<T> s1, IState<T> s2)
    {
        return s1 != null && s2 != null && s1.GetType().Name.Equals(s2.GetType().Name);
    }
    public bool CompareState(IState<T> s1, string s2)
    {
        return s1 != null && s2 != null && s1.GetType().Name.Equals(s2);
    }
}
