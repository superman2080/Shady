using UnityEngine;
using UnityEngine.AI;

public class MeleePatrol : IState<Enemy>
{
    private Vector2[] patrolPos = new Vector2[3];
    private int idx = 0;

    public float TransitionTime { get; set; } = 0.5f;

    public void Start(Enemy caster)
    {
        caster.stat.SetDefault(StatType.MOVE_SPEED, 0.5f);
        patrolPos = caster.RandomReachablePosition(caster.transform.position, caster.recogDist, 3, 3);
        caster.navMesh.SetDestination(patrolPos[idx]);
        caster.targetPos = patrolPos[idx];
        caster.isLookAtTarget = true;

        caster.spriteRenderer.color = Color.green;
    }

    public void Update(Enemy caster)
    {
        if(caster.HasReachedDestination(patrolPos[idx]))
        {
            idx = idx < patrolPos.Length - 1 ? idx + 1 : 0;
            caster.navMesh.SetDestination(patrolPos[idx]);
            caster.targetPos = patrolPos[idx];
        }
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
        {
            caster.stateMachine.ChangeState(new MeleeEngagement(), 0.5f);
        }    
    }

    public void Finish(Enemy caster)
    {
        Debug.LogWarning("End Patrol");
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
