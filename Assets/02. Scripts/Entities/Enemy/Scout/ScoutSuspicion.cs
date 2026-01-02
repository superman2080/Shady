using UnityEngine;

public class ScoutSuspicion : StateBase<Enemy>
{
    private Vector2 suspicionPos;
    private Vector2 originPos;
    private bool hasReached;

    public ScoutSuspicion(Vector2 suspicionPos)
    {
        this.suspicionPos = suspicionPos;
    }

    public override void Enter(Enemy caster)
    {
        originPos = caster.transform.position;
        caster.Stat.SetDefault(DefaultStatType.MOVE_SPEED, 2f);
        caster.navMesh.isStopped = false;
        caster.isLookAtTarget = true;
    }

    public override void Execute(Enemy caster)
    {
    }

    public override void Exit(Enemy caster)
    {
    }
}
