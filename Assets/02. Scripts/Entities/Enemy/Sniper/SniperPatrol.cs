using System;
using UnityEngine;

public class SniperPatrol : IState<Enemy>
{
    private Vector2 targetDir;
    private Sniper owner;
    private int idx = 0;

    public void Enter(Enemy caster)
    {
        caster.isLookAtTarget = true;
        caster.rotationSpeed = 30f;
        owner = caster as Sniper;
        targetDir = owner.patrolAreaEdge[idx];
        caster.targetPos = targetDir;
    }

    public void Execute(Enemy caster)
    {
        if(caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.AttackLayer))
        {
            caster.stateMachine.ChangeState(new SniperEngagement(), 0.05f);
        }
        if(GameMath.IsLookingDir(caster.transform, (owner.patrolAreaEdge[idx] - (Vector2)caster.transform.position).normalized, 1f))
        {
            idx = idx == 0 ? 1 : 0;
            targetDir = owner.patrolAreaEdge[idx];
            caster.targetPos = targetDir;
            caster.isLookAtTarget = true;
        }
    }
    public void Exit(Enemy caster)
    {
        caster.isLookAtTarget = false;
    }
}
