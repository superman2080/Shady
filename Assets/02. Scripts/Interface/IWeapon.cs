using UnityEngine;

public class WeaponStat
{

}

public interface IWeapon
{
    public LayerMask AttackLayer { get; }

    public void InitWeapon(Entity user);

    public void Using(Entity user);

}
