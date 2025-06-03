using UnityEngine;

public interface IWeapon
{
    public LayerMask AttackLayer { get; }

    public WeaponStat weaponStat { get; }

    public void InitWeapon(IAttackable user);

    public void Using(IAttackable user);

}
