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

    public void Start(Enemy caster)
    {
        canMove = caster.entityStat.Get(EntityStatType.MOVE_SPEED) > 0;
        originPos = caster.transform.position;
        caster.targetPos = targetPos;
        caster.isLookAtTarget = true;
        caster.navMesh.destination = targetPos;
        timer = new Timer(3f, () =>
        {
            caster.targetPos = originPos;
            caster.navMesh.destination = originPos;
            hasReached = true;
            Debug.Log("End");
        });
    }

    public void Update(Enemy caster)
    {
        if (canMove)
        {
            if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
                caster.stateMachine.ChangeStateImmediately(caster.AttackState);

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
            float dot = Vector2.Dot(caster.transform.right, (targetPos - originPos).normalized);
            if(dot >= 0.99f)
            {
                timer.Update(Time.deltaTime);
            }
            if(timer.IsRunning == false)
                caster.stateMachine.ChangeStateImmediately(caster.DefaultState);
        }
    }

    public void Finish(Enemy caster)
    {
        caster.navMesh.ResetPath();
    }
}
