using UnityEngine;

public class Doubt : StateBase<Enemy>
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

    public override void Enter(Enemy caster)
    {
        canMove = caster.Stat.Get(DefaultStatType.MOVE_SPEED) > 0;
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
                caster.stateMachine.ChangeState(caster.DefaultState);
            }, null, false);
        }
    }

    public override void Execute(Enemy caster)
    {
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
            caster.stateMachine.ChangeState(caster.AttackState);
        if (canMove)
        {
            if (caster.HasReachedDestination(targetPos))
            {
                caster.navMesh.ResetPath();
            
                timer.Update(Time.deltaTime);
            }
            if (caster.HasReachedDestination(originPos) && hasReached)
                caster.stateMachine.ChangeState(caster.DefaultState);
        }
        else
        {
            if(GameMath.IsLookingDir(caster.transform, (targetPos - originPos).normalized, 3))
            {
                timer.Update(Time.deltaTime);
            }
        }
    }

    public override void Exit(Enemy caster)
    {
        caster.navMesh.ResetPath();
    }
}
