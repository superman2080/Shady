using UnityEngine;

public class Unstoppable : StatusEffect
{
    public Unstoppable(int level, float duration, Entity caster = null) : base(level, duration, caster)
    {
    }

    public override StatusEffectType Type => StatusEffectType.BUFF;

    public override void OnEnter(Entity target)
    {
    }

    public override void OnExit(Entity target)
    {
    }

    public override void OnExecute(Entity target)
    {
    }
}
