using UnityEngine;

public class Doubt : IState<Enemy>
{

    private Vector2 targetPos;
    private Vector2 originPos;
    private bool hasReached;

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
    }

    public void Update(Enemy caster)
    {

        if (caster.HasReachedDestination(targetPos) && !hasReached)
        {
            hasReached = true;
            Timer timer = new Timer(3f, () =>
            {
                caster.navMesh.destination = originPos;
            });
            timer.Update(Time.deltaTime);
        }

        if (caster.HasReachedDestination(originPos) && hasReached)
            caster.stateMachine.ChangeStateImmediately(caster.DefaultState);
    }

    public void Finish(Enemy caster)
    {
    }
}
