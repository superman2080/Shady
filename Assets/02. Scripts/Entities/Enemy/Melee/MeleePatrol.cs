using UnityEngine;
using UnityEngine.AI;

public class MeleePatrol : StateBase<Enemy>
{
    private Vector2[] patrolPos = new Vector2[3];
    private int idx = 0;

    public override void Enter(Enemy caster)
    {
        caster.Stat.SetDefault(DefaultStatType.MOVE_SPEED, 1f);
        patrolPos = caster.RandomReachablePosition(caster.transform.position, caster.recogDist, 3, 3);
        caster.navMesh.SetDestination(patrolPos[idx]);
        caster.targetPos = patrolPos[idx];
        caster.isLookAtTarget = false;
        caster.rotationSpeed = 150f;

        caster.spriteRenderer.color = Color.green;
    }

    public override void Execute(Enemy caster)
    {
        if(caster.HasReachedDestination(patrolPos[idx]))
        {
            idx = idx < patrolPos.Length - 1 ? idx + 1 : 0;
            caster.navMesh.SetDestination(patrolPos[idx]);
            caster.targetPos = patrolPos[idx];
        }
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
        {
            caster.stateMachine.ChangeState(new MeleeEngagement());
        }    
    }

    public override void Exit(Enemy caster)
    {
        Debug.LogWarning("End Patrol");
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
