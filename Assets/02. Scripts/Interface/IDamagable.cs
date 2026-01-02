using UnityEngine;

public interface IDamagable 
{
    public DefaultStat Stat { get; }

    public void TakeDamage(IAttackable caster, float amount);

    public void Heal(Entity caster, float amount);
}
