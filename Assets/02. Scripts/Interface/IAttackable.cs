using UnityEngine;

public interface IAttackable
{
    public WeaponCtrl WeaponController { get; } 
    public void Attack(Entity caster, float amount);
    public void OnEntityAttack(Entity caster, float amount);
    public LayerMask AttackLayer { get; }
    public bool CanAttack { get; }
}
