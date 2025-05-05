using UnityEngine;

public interface IDamagable
{
    public float HP { get; }
    public void TakeDamage(Entity caster, float amount);

    public void Heal(Entity caster, float amount);
}
