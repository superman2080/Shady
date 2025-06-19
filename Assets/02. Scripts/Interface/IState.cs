using UnityEngine;

public interface IState<T>
{
    public void Enter(T caster);

    public void Execute(T caster);

    public void Exit(T caster);
}
