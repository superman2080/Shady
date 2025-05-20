using UnityEngine;

public interface IAttackable
{
    public WeaponCtrl WeaponController { get; } 
    public Coroutine AttackTimerCor { get; }
    public void Attack(Entity caster, float amount);
}
