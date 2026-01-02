using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Dagger : IWeapon
{
    public LayerMask AttackLayer { get; private set; }

    public void EquipWeapon(IAttackable user)
    {
        AttackLayer = user.AttackLayer;

        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 2);
        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 2);
        user.WeaponStat.SetDefault(WeaponStatType.DAMAGE, 20f);
    }

    public void UnequipWeapon(IAttackable user)
    {

    }

    public void Using(IAttackable user)
    {
        var tr = (user as MonoBehaviour).transform;
        Vector2 origin = tr.position;
        float range = user.WeaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

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
                    entity.TakeDamage(user, user.WeaponStat.Get(WeaponStatType.DAMAGE));
                }
            }
        }
    }
}
