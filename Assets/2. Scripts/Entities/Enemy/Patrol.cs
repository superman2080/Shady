using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Patrol : IState<Enemy>
{
    private List<Vector2> patrolPos;
    private int idx = 0;

    public void Start(Enemy caster)
    {
        patrolPos = new List<Vector2>();
        
        for (int i = 0; i < 3; i++)
        {
            patrolPos.Add(caster.RandomReachablePosition(caster.recogDist));
        }
        caster.navMesh.SetDestination(patrolPos[idx]);
        caster.targetPos = patrolPos[idx];
        caster.isLookAtTarget = true;
    }

    public void Update(Enemy caster)
    {
        if(caster.HasReachedDestination(patrolPos[idx]))
        {
            idx = idx < patrolPos.Count - 1 ? idx + 1 : 0;
            caster.navMesh.SetDestination(patrolPos[idx]);
            caster.targetPos = patrolPos[idx];
        }
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
        {
            Vector2 targetPos = caster.FieldOfView(caster.recogDist, caster.sightAngle, caster.recogLayer).Find(obj => obj.TryGetComponent(out PlayerCtrl player)).transform.position;
            caster.targetPos = targetPos;
            caster.stateMachine.ChangeState(new Suspicion());
        }    
    }

    public void Finish(Enemy caster)
    {
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
