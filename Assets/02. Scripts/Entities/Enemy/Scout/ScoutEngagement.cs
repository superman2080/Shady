using UnityEngine;

public class ScoutEngagement : IState<Enemy>
{
    private Vector2 targetPos;

    public void Enter(Enemy caster)
    {
        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 5);
        targetPos = caster.RandomReachablePosition(10f);
        caster.navMesh.destination = targetPos;
        caster.isLookAtTarget = true;
        caster.targetPos = targetPos;
    }

    public void Execute(Enemy caster)
    {
        caster.Attack(caster, caster.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        if (caster.HasReachedDestination(targetPos))
        {
            targetPos = caster.RandomReachablePosition(10f);
            caster.navMesh.destination = targetPos;
            caster.targetPos = targetPos;
        }
    }

    public void Exit(Enemy caster)
    {
    }
}
