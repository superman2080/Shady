using UnityEngine;

public delegate void OnAttack(Entity caster, float amount);
public interface IAttackable
{
    public WeaponCtrl WeaponController { get; } 
    public AttackStat WeaponStat { get; }
    public LayerMask AttackLayer { get; }
    public void Attack(Entity caster, float amount);

    public event OnAttack OnAttackEvent;

}
