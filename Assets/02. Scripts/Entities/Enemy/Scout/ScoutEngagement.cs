using UnityEngine;

public class ScoutEngagement : StateBase<Enemy>
{
    private Vector2 targetPos;

    public override void Enter(Enemy caster)
    {
        caster.Stat.SetDefault(DefaultStatType.MOVE_SPEED, 8);
        targetPos = caster.RandomReachablePosition(10f);
        caster.navMesh.destination = targetPos;
        caster.isLookAtTarget = true;
        caster.targetPos = targetPos;
    }

    public override void Execute(Enemy caster)
    {
        caster.Attack(caster, caster.WeaponStat.Get(WeaponStatType.DAMAGE));
        if (caster.HasReachedDestination(targetPos))
        {
            targetPos = caster.RandomReachablePosition(10f);
            caster.navMesh.destination = targetPos;
            caster.targetPos = targetPos;
        }
    }

    public override void Exit(Enemy caster)
    {
    }
}
