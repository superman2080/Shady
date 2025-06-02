using UnityEngine;

public class MeleeRetreat : IState<Enemy>
{
    public float TransitionTime { get; set; } = 0.5f;

    public void Start(Enemy caster)
    {
    }

    public void Update(Enemy caster)
    {
    }

    public void Finish(Enemy caster)
    {
    }
}
