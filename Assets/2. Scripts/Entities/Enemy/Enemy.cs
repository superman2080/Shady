#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : Entity
{
    public StateMachine<Enemy>? stateMachine;
    public float recogDist;
    public float engTime;
    [Range(30, 90)] public float sightAngle;
    private int recogLayer;


    protected override void Start()
    {
        base.Start();
        stateMachine = new StateMachine<Enemy>(this);
        stateMachine.ChangeState(new Patrol());

        recogLayer = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("ScanTile")) | (1 << LayerMask.NameToLayer("Shadow"));
    }

    protected virtual void Update()
    {
        stateMachine?.Update();
    }

    protected List<GameObject>? FieldOfView(float range, float angle, int layer)
    {
        Vector2 origin = transform.position;
        List<GameObject> result = new List<GameObject>();
        Collider2D[] targets = Physics2D.OverlapCircleAll(origin, range, layer);
        if (targets.Length <= 0)
            return null;
        else
        {
            foreach (var target in targets)
            {
                Vector2 targetPos = target.transform.position;
                Vector2 dir = (targetPos - origin).normalized;
                float theta = Mathf.Acos(Vector3.Dot(transform.right, dir)) * Mathf.Rad2Deg;

                if(Physics2D.Raycast(origin, dir, range, layer).collider == target && theta <= angle)
                {
                    result.Add(target.gameObject);
                }
            }
        }
        return result;
    }

    //public EnemyState GetEnemyState(EnemyState e)
    //{
    //    List<GameObject> objList = FieldOfView(recogDist, sightAngle, recogLayer);
    //    switch (e)
    //    {
    //        case EnemyState.Patrol:
    //            if(objList.Exists(o => o.TryGetComponent(out PlayerCtrl p) == true))
    //            {
    //                return EnemyState.Suspiocion;
    //            }
    //            else
    //            {
    //                return EnemyState.Patrol;
    //            }
    //        case EnemyState.Suspiocion:

    //            break;
    //        case EnemyState.Engagement:
    //            break;
    //        default:
    //            break;
    //    }
    //}

    private void OnDrawGizmos()
    {

        Vector2 origin = transform.position;
        float angle = transform.eulerAngles.z;
        Vector2 leftSight = new Vector2(Mathf.Cos((angle + sightAngle) * Mathf.Deg2Rad), Mathf.Sin((angle + sightAngle) * Mathf.Deg2Rad)).normalized;
        Vector2 rightSight = new Vector2(Mathf.Cos((angle - sightAngle) * Mathf.Deg2Rad), Mathf.Sin((angle - sightAngle) * Mathf.Deg2Rad)).normalized;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, recogDist);


        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, leftSight * recogDist);
        Gizmos.DrawRay(origin, rightSight * recogDist);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, transform.right * recogDist);
    }

    protected override void OnEntityDied(Entity caster)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnTakeDamage(Entity caster, float amount)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
        throw new System.NotImplementedException();
    }
}
