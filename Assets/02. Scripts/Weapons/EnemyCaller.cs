using System.Linq;
using UnityEngine;
using PlayerNameSpace;

public class EnemyCaller : IWeapon
{
    public LayerMask AttackLayer { get => 1 << LayerMask.NameToLayer("Enemy"); }

    public void EquipWeapon(IAttackable user)
    {
        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_DISTANCE, 10);
        user.WeaponStat.SetDefault(WeaponStatType.DAMAGE, 0);
        user.WeaponStat.SetDefault(WeaponStatType.ATTACK_SPEED, 0.2f);
    }

    public void UnequipWeapon(IAttackable user)
    {
    }

    public void Using(IAttackable user)
    {
        Vector2 origin = (user as MonoBehaviour).transform.position;
        Vector2 playerPos = Object.FindAnyObjectByType<Player>().transform.position;
        Doubt taunt = new Doubt(GameMath.GetOffsetPosition(origin, playerPos, 2));
        float dist = user.WeaponStat.Get(WeaponStatType.ATTACK_DISTANCE);

        Enemy[] enemies = Physics2D.OverlapCircleAll(origin, dist, AttackLayer).Select(col => col.GetComponent<Enemy>()).ToArray();
        if (enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.stateMachine.CurState?.GetType().Name == enemy.DefaultState.GetType().Name)
                {
                    Debug.Log($"{enemy.gameObject.name}: Doubt");
                    enemy.stateMachine.ChangeState(taunt);
                }
            }
        }
    }
}
