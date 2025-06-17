using System.Linq;
using UnityEngine;

public class EnemyCaller : IWeapon
{
    public LayerMask AttackLayer { get => 1 << LayerMask.NameToLayer("Enemy"); }

    public void InitWeapon(IAttackable user)
    {
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 10);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.DAMAGE, 0);
        user.WeaponController.weaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 0.2f);
    }

    public void Using(IAttackable user)
    {
        Vector2 origin = (user as MonoBehaviour).transform.position;
        Vector2 playerPos = Object.FindAnyObjectByType<PlayerCtrl>().transform.position;
        Doubt taunt = new Doubt(GameMath.GetOffsetPosition(origin, playerPos, 2));
        float dist = user.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

        Enemy[] enemies = Physics2D.OverlapCircleAll(origin, dist, AttackLayer).Select(col => col.GetComponent<Enemy>()).ToArray();
        if (enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.stateMachine.State?.GetType().Name == enemy.DefaultState.GetType().Name)
                {
                    Debug.Log($"{enemy.gameObject.name}: Doubt");
                    enemy.stateMachine.ChangeStateImmediately(taunt);
                }
            }
        }
    }
}
