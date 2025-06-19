using UnityEngine;

public class MeleeRetreat : IState<Enemy>
{
    public float TransitionTime { get; set; } = 0.5f;

    public void Enter(Enemy caster)
    {
    }

    public void Execute(Enemy caster)
    {
    }

    public void Exit(Enemy caster)
    {
    }
}
