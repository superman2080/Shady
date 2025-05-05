using UnityEngine;

public class Enemy : Entity
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEntityDied(Entity caster)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnTakeDamage(Entity caster, float amount)
    {
    }
}
