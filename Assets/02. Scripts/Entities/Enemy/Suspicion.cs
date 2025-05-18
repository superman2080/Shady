using UnityEngine;


/*
 * This state is a state where you first move to a suspected location, 
 * then look at a specific point in the radius, 
 * and then return to the combat state if you see the player for a certain period of time or if you can't see it.
*/
public class Suspicion : IState<Enemy>
{
    private Vector2 originPos;
    private Vector2 suspicionPos;
    private Vector2[] searchPos = new Vector2[3];
    private float detectTime = 0;
    private int idx = 0;

    public void Start(Enemy caster)
    {
        Debug.LogWarning("Start Suspicion");
        originPos = caster.transform.position;
        caster.navMesh.SetDestination(suspicionPos);
        suspicionPos = caster.targetPos;
        searchPos = caster.RandomReachablePosition(suspicionPos, caster.searchRange, 3, 1);
    }

    public void Update(Enemy caster)
    {
        if(caster.IsPlayerInSight(caster.recogDist, caster.sightAngle, caster.recogLayer) == true)
        {
            Vector2 playerPos = caster.FieldOfView(caster.recogDist, caster.sightAngle, caster.recogLayer).Find(obj => obj.TryGetComponent(out PlayerCtrl player)).transform.position;
            detectTime += Time.deltaTime;
            caster.navMesh.destination = playerPos;
            caster.targetPos = playerPos;
            if (detectTime >= caster.engageTime)
            {
                caster.stateMachine.ChangeState(new Engagement());
                Debug.LogWarning("Has engaged");
            }
        }
        else
        {
            if (detectTime > -caster.searchTime)
            {
                detectTime -= Time.deltaTime;
                caster.targetPos = searchPos[idx];
                caster.navMesh.destination = searchPos[idx];
            }
            else
            {
                caster.targetPos = originPos;
                caster.navMesh.destination = originPos;
                if (caster.HasReachedDestination(originPos))
                    caster.stateMachine.ChangeState(new Patrol());
            }

            if (caster.HasReachedDestination(searchPos[idx]))
            {
                idx = idx < searchPos.Length - 1 ? idx + 1 : 0;
            }
        }
    }

    public void Finish(Enemy caster)
    {
        Debug.LogWarning("End Suspicion");
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
