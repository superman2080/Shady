using UnityEngine;

public class ScoutSuspicion : IState<Enemy>
{
    private Vector2 suspicionPos;
    private Vector2 originPos;
    private bool hasReached;

    public ScoutSuspicion(Vector2 suspicionPos)
    {
        this.suspicionPos = suspicionPos;
    }

    public void Enter(Enemy caster)
    {
        originPos = caster.transform.position;
        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 2f);
        caster.navMesh.isStopped = false;
        caster.isLookAtTarget = true;
    }

    public void Execute(Enemy caster)
    {
    }

    public void Exit(Enemy caster)
    {
    }
}
