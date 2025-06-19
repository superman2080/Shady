using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoutPatrol : IState<Enemy>
{
    private Vector2[] patrolPos = new Vector2[5];
    private int idx = 0;

    private List<Vector2> lateLightPosList = new List<Vector2>();
    private List<Shadow> lateShadowList = new List<Shadow>();
    private List<Vector2> lateShadowPosList = new List<Vector2>();
    private bool lateIsInShadow;

    public void Enter(Enemy caster)
    {
        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 3f);
        patrolPos = caster.RandomReachablePosition(caster.transform.position, caster.recogDist, 10, 5);
        caster.navMesh.SetDestination(patrolPos[idx]);
        caster.targetPos = patrolPos[idx];
        caster.isLookAtTarget = false;
        caster.rotationSpeed = 180f;
        caster.spriteRenderer.color = Color.green;
    }

    public void Execute(Enemy caster)
    {
        if (caster.HasReachedDestination(patrolPos[idx]))
        {
            idx = idx < patrolPos.Length - 1 ? idx + 1 : 0;
            caster.navMesh.SetDestination(patrolPos[idx]);
            caster.targetPos = patrolPos[idx];
        }
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
        {
            caster.stateMachine.ChangeState(new ScoutEngagement(), 0.5f);
        }

        FindNewLightSource(caster);

        HasDifferentShadowInSight(caster);
    }

    public void Exit(Enemy caster)
    {
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }

    private void FindNewLightSource(Enemy caster)
    {
        var lightList = caster.FieldOfView(caster.recogDist, caster.sightAngle, 1 << LayerMask.NameToLayer("Light"))?.Select((s) => s.GetComponent<ShadowCaster>()).ToList();
        if (lightList != null)
        {
            foreach (var light in lightList)
            {
                if (light.activatedTime > 0.1f)
                {
                    caster.stateMachine.ChangeState(new Doubt(light.transform.position), 0.5f);
                    Debug.LogError("Find new light!");
                }
            }
        }
    }

    private void HasDifferentShadowInSight(Enemy caster)
    {
        Vector2 origin = caster.transform.position;

        var curShadowList = caster.FieldOfView<Shadow>(caster.recogDist, caster.sightAngle, 1 << LayerMask.NameToLayer("Shadow"))?.ToList() ?? new List<Shadow>();
        var curLightPosList = curShadowList.Select(o => (Vector2)o.lightSource.transform.position).Distinct().ToList();

        // Changed shadow shape
        if(curLightPosList.Count > 0 && lateLightPosList.Count > 0 && curLightPosList.Count == lateLightPosList.Count)
        {
            var diff = curLightPosList.Except(lateLightPosList).FirstOrDefault();
            if(diff != default)
            {
                Debug.LogError("Changed shadow shape");
                caster.stateMachine.ChangeState(new Doubt(GameMath.GetOffsetPosition(origin, diff, 2)), 0.5f);
                return;
            }
        }
        // Find a new shadow in sight
        if (curShadowList.Count > 0 && curShadowList.Count > lateShadowList.Count)
        {
            Shadow except = curShadowList.Except(lateShadowList).First();

            if (except.lightSource.activatedTime > 0.1f && except.lightSource.activatedTime >= Time.time)
            {
                caster.stateMachine.ChangeState(new Doubt(GameMath.GetOffsetPosition(origin, except.lightSource.transform.position, 2)), 0.5f);
                Debug.LogError("Find a new shadow in sight");
                return;
            }
        }
        //Disappear shadow in sight
        else if (lateShadowList.Count > 0 && lateShadowList.Count > curShadowList.Count)
        {
            foreach (var shadow in lateShadowList)
            {
                if(!curShadowList.Exists(s => s == shadow) && caster.IsInShadow().isIn == lateIsInShadow && lateShadowList.Find(o => o == shadow).lightSource == null)
                {
                    int idx = lateShadowList.FindIndex(o => o == shadow);
                    caster.stateMachine.ChangeState(new Doubt(GameMath.GetOffsetPosition(origin, lateShadowPosList[idx], 2)), 0.5f);
                    Debug.LogError("Disappear shadow in sight");
                    return;
                }
            }
        }
        lateShadowList = curShadowList.ToList();
        lateShadowPosList = curShadowList.Select(t => (Vector2)t.lightSource.transform.position).ToList();
        lateIsInShadow = caster.IsInShadow().isIn;
        lateLightPosList = curLightPosList.ToList();
    }
}
