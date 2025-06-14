using UnityEngine;

public class Doubt : IState<Enemy>
{

    private Vector2 targetPos;
    private Vector2 originPos;
    private bool hasReached = false;
    private Timer timer;
    public Doubt(Vector2 targetPos)
    {
        this.targetPos = targetPos;
    }

    public void Start(Enemy caster)
    {
        originPos = caster.transform.position;
        caster.targetPos = targetPos;
        caster.navMesh.destination = targetPos;
        caster.isLookAtTarget = true;
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

    public void Finish(Enemy caster)
    {
        caster.navMesh.ResetPath();
    }
}
