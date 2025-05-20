using UnityEngine;

public class WeaponCtrl
{
    public Entity user { get; private set; }
    public IWeapon nowWeapon { get; private set; }
    private IWeapon lastWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public WeaponCtrl(Entity user, IWeapon weapon)
    {
        this.user = user;
        SetWeapon(weapon);
    }

    public void SetWeapon(IWeapon weapon)
    {
        nowWeapon = weapon;
        lastWeapon = nowWeapon;
        nowWeapon.InitWeapon(user);
    }

    public void UsingWeapon()
    {
        if(lastWeapon != nowWeapon)
        {
            nowWeapon.InitWeapon(user);
            lastWeapon = nowWeapon;
        }
        nowWeapon.Using(user);
    }
}
