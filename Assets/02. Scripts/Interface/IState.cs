using UnityEngine;

public interface IState<T>
{
    void Enter(T caster);

    void Execute(T caster);

    void FixedExecute(T caster);

    void Exit(T caster);
}
