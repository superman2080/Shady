using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SniperRifle : IWeapon
{
    public LayerMask AttackLayer { get; private set; }
    private SniperRifleProjectile projectilePrefab;

    public void InitWeapon(IAttackable user)
    {
        AttackLayer = user.AttackLayer;
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 10);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.DAMAGE, 35);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 1f);
        projectilePrefab = Resources.Load<SniperRifleProjectile>("Prefabs/HitScan/SniperRifleProjectile");
    }

    public void Using(IAttackable user)
    {
        var tr = (user as MonoBehaviour).transform;
        Vector2 dir = tr.right;
        var projectile = Object.Instantiate(projectilePrefab, tr);
        projectile.Fire(user, tr.position, dir);
    }
}
