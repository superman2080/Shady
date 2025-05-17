using UnityEngine;

public interface IState<T>
{
    public void Start(T caster);

    public void Update(T caster);

    public void Finish(T caster);
}
