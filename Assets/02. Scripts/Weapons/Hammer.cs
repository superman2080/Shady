using UnityEngine;

public class Hammer : IWeapon
{
    public LayerMask AttackLayer { get; private set; }

    public void EquipWeapon(IAttackable user)
    {
        AttackLayer = user.AttackLayer;

        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 0.75f);
        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 3);
        user.WeaponStat.SetDefault(WeaponStatType.DAMAGE, 50f);
    }

    public void UnequipWeapon(IAttackable user)
    {
        throw new System.NotImplementedException();
    }

    public void Using(IAttackable user)
    {
        new Timer(Mathf.Lerp(0, 1f / user.WeaponStat.Get(WeaponStatType.ATTACK_SPEED), 0.5f), () => Casting(user));
    }

    public void Casting(IAttackable user)
    {
        if ((user as MonoBehaviour) == null)
            return;
        var transform = (user as MonoBehaviour).transform;
        Vector2 origin = transform.position;
        Vector2 dir = transform.right;
        float range = user.WeaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

        Collider2D[] col = Physics2D.OverlapBoxAll(origin + dir * range, new Vector2(2, range), transform.eulerAngles.z, AttackLayer);
        foreach (var obj in col)
        {
            Debug.Log(obj.name);
            obj.GetComponent<IDamagable>().TakeDamage(user, user.WeaponStat.Get(WeaponStatType.DAMAGE));
        }
    }

}
