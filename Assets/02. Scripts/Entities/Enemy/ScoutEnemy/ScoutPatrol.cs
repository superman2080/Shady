using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoutPatrol : IState<Enemy>
{
    private Vector2[] patrolPos = new Vector2[5];
    private int idx = 0;
    private List<ShadowCaster> lightList = new List<ShadowCaster>();

    private ShadowCaster lateLight = null;
    private Vector2 curLightPos;
    private Vector2 lateLightPos;
    
    private bool lateIsInShadow;
    public void Start(Enemy caster)
    {
        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 3f);
        patrolPos = caster.RandomReachablePosition(caster.transform.position, caster.recogDist, 10, 5);
        caster.navMesh.SetDestination(patrolPos[idx]);
        caster.targetPos = patrolPos[idx];
        caster.isLookAtTarget = true;
        caster.rotationSpeed = 180f;
        caster.spriteRenderer.color = Color.green;
    }

    public void Update(Enemy caster)
    {
        if (caster.HasReachedDestination(patrolPos[idx]))
        {
            idx = idx < patrolPos.Length - 1 ? idx + 1 : 0;
            caster.navMesh.SetDestination(patrolPos[idx]);
            caster.targetPos = patrolPos[idx];
        }
        if (caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer))
        {
            Vector2 playerPos = caster.playerTr.position;
            Vector2 origin = caster.transform.position;
            Vector2 targetPos = playerPos - (origin - playerPos).normalized * -2;
            caster.stateMachine.ChangeState(new Doubt(targetPos), 0.5f);
        }

        // To compare new light in sight
        lightList = caster.FieldOfView(caster.recogDist, caster.sightAngle, 1 << LayerMask.NameToLayer("Light"))?.Select((s) => s.GetComponent<ShadowCaster>()).ToList();
        if(lightList != null)
        {
            foreach (var light in lightList)
            {
                if(light.activatedTime > 0.1f) {
                    caster.stateMachine.ChangeState(new Doubt(light.transform.position), 0.5f);
                    Debug.LogError("Find new light!");
                }
            }
        }

        // When there's a shadow change
        if (caster.IsInShadow().isIn)
        {
            // When you go into another shadow
            if (caster.IsInShadow().col.GetComponent<Shadow>().lightSource != lateLight)
            {
                lateLight = caster.IsInShadow().col.GetComponent<Shadow>().lightSource;
                lateLightPos = curLightPos;
            }
            curLightPos = caster.IsInShadow().col.GetComponent<Shadow>().lightSource.transform.position;
            Debug.Log(caster.IsInShadow().isIn != lateIsInShadow);
            if (Time.time > 0.1f && caster.IsInShadow().isIn != lateIsInShadow && (curLightPos - lateLightPos).sqrMagnitude >= 0.0001f)
                caster.stateMachine.ChangeState(new Doubt(curLightPos), 0.5f);
        }
        lateLightPos = curLightPos;
        lateIsInShadow = caster.IsInShadow().isIn;
    }

    public void Finish(Enemy caster)
    {
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
