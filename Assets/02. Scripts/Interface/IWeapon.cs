using UnityEngine;

public interface IWeapon
{
    public LayerMask AttackLayer { get; }

    public void EquipWeapon(IAttackable user);

    public void UnequipWeapon(IAttackable user);

    public void Using(IAttackable user);

}
