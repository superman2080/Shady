using UnityEngine;

public class StateMachine<T>
{
    private IState<T> state;
    private T caster;


    public StateMachine(T caster, IState<T> state)
    {
        this.caster = caster;
        this.state = state;
        this.state.Start(caster);
    }

    public void ChangeState(IState<T> newState)
    {
        if (state == newState) return;

        state.Finish(caster);
        state = newState;
        state.Start(caster);
    }

    public void Update()
    {
        state.Update(caster);
    }
}
