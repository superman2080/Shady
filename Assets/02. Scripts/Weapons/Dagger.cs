using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Dagger : IWeapon
{
    public LayerMask AttackLayer { get; private set; }

    public WeaponStat weaponStat { get; private set; } = new WeaponStat();

    public void InitWeapon(IAttackable user)
    {
        AttackLayer = user.AttackLayer;

        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 2);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 2);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.DAMAGE, 20f);
    }

    public void Using(IAttackable user)
    {
        var tr = (user as MonoBehaviour).transform;
        Vector2 origin = tr.position;
        float range = user.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

        Collider2D[] col = Physics2D.OverlapCircleAll(tr.position, range, AttackLayer);
        if (col.Length <= 0)
            return;
        else
        {
            foreach (var obj in col)
            {
                Vector2 targetPos = obj.transform.position;
                Vector2 dir = (targetPos - origin).normalized;
                float theta = Mathf.Acos(Vector3.Dot(tr.right, dir)) * Mathf.Rad2Deg;

                if (Physics2D.Raycast(origin, dir, range, AttackLayer).collider == obj && theta <= 90 && obj.TryGetComponent(out IDamagable entity))
                {
                    entity.TakeDamage(user, user.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
                    Debug.LogWarning(entity.HP);
                }
            }
        }
    }
}
