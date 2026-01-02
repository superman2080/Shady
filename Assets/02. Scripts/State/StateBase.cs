using UnityEngine;

public abstract class StateBase<T> : IState<T> where T : MonoBehaviour
{
    public virtual void Enter(T caster)
    {
    }

    public virtual void Execute(T caster)
    {
    }

    public virtual void FixedExecute(T caster)
    {
    }

    public virtual void Exit(T caster)
    {
    }
}

public abstract class MonoStateBase<T> : MonoBehaviour, IState<T> where T : MonoBehaviour
{
    public virtual void Enter(T caster)
    {
    }

    public virtual void Execute(T caster)
    {
    }
    public virtual void FixedExecute(T caster)
    {
    }

    public virtual void Exit(T caster)
    {
    }

}
