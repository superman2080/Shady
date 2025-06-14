using UnityEngine;

public class Hammer : IWeapon
{
    public LayerMask AttackLayer { get; private set; }

    public void InitWeapon(IAttackable user)
    {
        AttackLayer = user.AttackLayer;

        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 0.75f);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 3);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.DAMAGE, 50f);
    }

    public void Using(IAttackable user)
    {
        var transform = (user as MonoBehaviour).transform;
        Vector2 origin = transform.position;
        Vector2 dir = transform.right;
        float range = user.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

        Collider2D[] col = Physics2D.OverlapBoxAll(origin + dir * range, new Vector2(2, range), transform.eulerAngles.z, AttackLayer);
        foreach (var obj in col)
        {
            Debug.Log(obj.name);
            obj.GetComponent<IDamagable>().TakeDamage(user, user.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        }
    }
}
