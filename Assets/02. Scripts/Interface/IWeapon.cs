using UnityEngine;

public interface IWeapon
{
    public LayerMask AttackLayer { get; }

    public void InitWeapon(Entity user);

    public void Using(Entity user);

}
