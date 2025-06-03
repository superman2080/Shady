using UnityEngine;

public class WeaponCtrl
{
    public Entity user { get; private set; }
    public WeaponStat weaponStat { get; private set; }
    public IWeapon nowWeapon { get; private set; }
    private IWeapon lastWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public WeaponCtrl(Entity user)
    {
        weaponStat = new WeaponStat();
        this.user = user;
    }

    public void SetWeapon(IWeapon weapon)
    {
        if (nowWeapon == weapon)
            return;
        nowWeapon = weapon;
        lastWeapon = nowWeapon;
        nowWeapon.InitWeapon(user as IAttackable);
    }

    public void UsingWeapon()
    {
        if (lastWeapon != nowWeapon)
        {
            nowWeapon.InitWeapon(user as IAttackable);
            lastWeapon = nowWeapon;
        }
        nowWeapon.Using(user as IAttackable);
    }
}
