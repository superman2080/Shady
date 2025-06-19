using UnityEngine;

public class Doubt : IState<Enemy>
{

    private Vector2 targetPos;
    private Vector2 originPos;
    private bool hasReached = false;
    private Timer timer;
    private bool canMove;

    public Doubt(Vector2 targetPos)
    {
        this.targetPos = targetPos;
    }

    public void Enter(Enemy caster)
    {
        canMove = caster.entityStat.Get(EntityStatType.MOVE_SPEED) > 0;
        caster.targetPos = targetPos;
        caster.isLookAtTarget = true;
        if (canMove)
        {
            originPos = caster.transform.position;
            caster.navMesh.destination = targetPos;
            timer = new Timer(3f, () =>
            {
                caster.targetPos = originPos;
                caster.navMesh.destination = originPos;
                hasReached = true;
            }, null, false);
        }
        else
        {
            timer = new Timer(5f, () =>
            {
                caster.stateMachine.ChangeStateImmediately(caster.DefaultState);
            }, null, false);
        }
    }

    public void Execute(Enemy caster)
    {
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
            caster.stateMachine.ChangeStateImmediately(caster.AttackState);
        if (canMove)
        {
            if (caster.HasReachedDestination(targetPos))
            {
                caster.navMesh.ResetPath();
            
                timer.Update(Time.deltaTime);
            }
            if (caster.HasReachedDestination(originPos) && hasReached)
                caster.stateMachine.ChangeStateImmediately(caster.DefaultState);
        }
        else
        {
            if(GameMath.IsLookingDir(caster.transform, (targetPos - originPos).normalized, 3))
            {
                timer.Update(Time.deltaTime);
            }
        }
    }

    public void Exit(Enemy caster)
    {
        caster.navMesh.ResetPath();
    }
}
