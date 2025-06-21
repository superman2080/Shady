#nullable enable
#pragma warning disable CS8602
#pragma warning disable CS8625
#pragma warning disable CS8618
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class Enemy : Entity, IAttackable
{
    [HideInInspector] public StateMachine<Enemy>? stateMachine;
    [HideInInspector] public NavMeshAgent? navMesh;
    [HideInInspector] public Vector2 targetPos;

    //[Header("Related to suspicion state")]
    //public float searchRange;
    public bool isLookAtTarget { get; set; } = true;
    public float rotationSpeed = 150f;

    [Header("Related to sight")]
    [Min(0.5f)] public float recogDist;
    [Range(10, 180)] public float sightAngle;
    public LayerMask recogLayer;
    public Transform? playerTr { get; private set; }
    #region AuditoryAttribute
    public AnimationCurve listenPercentage = AnimationCurve.Linear(0, 1, 1, 0);
    [Min(0.5f)] public float listenDist;
    private Vector2 latePlayerPos;
    #endregion

    public abstract IState<Enemy> DefaultState { get; }
    public abstract IState<Enemy> AttackState { get; }


    #region IAttackable

    private EnemyHUD hud;
    public Coroutine AttackTimerCor { get; protected set; } = null;


    public WeaponCtrl WeaponController { get; protected set; }

    [SerializeField] public LayerMask AttackLayer { get => 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Player"); }
    #endregion



    protected override void Start()
    {
        base.Start();
        playerTr = FindAnyObjectByType<PlayerCtrl>().transform;
        latePlayerPos = playerTr.transform.position;

        navMesh = gameObject.GetComponent<NavMeshAgent>();
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
        navMesh.angularSpeed = rotationSpeed;

        WeaponController = new WeaponCtrl(this);
        stateMachine = new StateMachine<Enemy>(this, DefaultState);
    }

    protected virtual void Update()
    {
        Vector2 playerPos = playerTr.transform.position;
        navMesh.isStopped = !canBehavior;
        if (canBehavior)
        {
            stateMachine?.Update();
            LookAtTarget(isLookAtTarget, rotationSpeed);
            navMesh.speed = entityStat.Get(EntityStatType.MOVE_SPEED);
            #region Listening Step
            float step = 0.5f;
            if (((Vector2)transform.position - playerPos).magnitude <= listenDist && (playerPos - latePlayerPos).magnitude >= step)
            {
                latePlayerPos = playerPos;
                if (HasAuditoryDetection())
                    OnAuditoryDectected(playerPos);
            }
            #endregion
        }
        HUD();
    }

    private void HUD()
    {
        if (Util.IsVisibleFromCamera(Camera.main, transform) && InGameUI.Instance.hudPool.HasHUD(this) == false)
            hud = InGameUI.Instance.hudPool.Get(this);
        else if (Util.IsVisibleFromCamera(Camera.main, transform) == false && InGameUI.Instance.hudPool.HasHUD(this) == true)
        {
            InGameUI.Instance.hudPool.Return(hud);
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        WeaponController.weaponStat.Update();
    }

    protected virtual void OnDestroy()
    {
        if(hud != null)
            InGameUI.Instance.hudPool.Return(hud);
    }

    public bool IsPlayerInSight(float range, float angle, int layer)
    {
        List<GameObject>? list = FieldOfView(range, angle / 2, layer);
        if (list == null)
            return false;
        return list.Exists(obj => obj.TryGetComponent(out PlayerCtrl player));
    }

    protected virtual void OnDrawGizmos()
    {
        Vector2 origin = transform.position;
        float angle = transform.eulerAngles.z;
        float sA = sightAngle / 2f;
        Vector2 leftSight = new Vector2(Mathf.Cos((angle + sA) * Mathf.Deg2Rad), Mathf.Sin((angle + sA) * Mathf.Deg2Rad)).normalized;
        Vector2 rightSight = new Vector2(Mathf.Cos((angle - sA) * Mathf.Deg2Rad), Mathf.Sin((angle - sA) * Mathf.Deg2Rad)).normalized;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, recogDist);


        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, leftSight * recogDist);
        Gizmos.DrawRay(origin, rightSight * recogDist);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, transform.right * recogDist);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(origin, listenDist);
    }

    public bool HasReachedDestination(Vector2 pos)
    {
        if (!navMesh.pathPending && navMesh.remainingDistance <= navMesh.stoppingDistance && (!navMesh.hasPath || navMesh.velocity.sqrMagnitude == 0f))
            return true;
        else
            return false;
    }

    public Vector2 RandomReachablePosition(float range)
    {
        if (navMesh == null)
            return Vector2.zero;
        int tryCnt = 0;
        while (tryCnt < 100)
        {
            Vector2 samplePos = (Vector2)transform.position + new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            var path = new NavMeshPath();
            if (navMesh.CalculatePath(samplePos, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                return samplePos;
            }
            tryCnt++;
        }
        return Vector2.zero;
    }

    public Vector2 RandomReachablePosition(Vector2 origin, float range)
    {
        if (navMesh == null)
            return Vector2.zero;
        int tryCnt = 0;
        while (tryCnt < 100)
        {
            Vector2 samplePos = origin + new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            var path = new NavMeshPath();
            if (navMesh.CalculatePath(samplePos, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                return samplePos;
            }
            tryCnt++;
        }
        return Vector2.zero;
    }

    public Vector2[] RandomReachablePosition(Vector2 origin, float range, int len, float interval)
    {
        Vector2[] result = new Vector2[len];
        for (int i = 0; i < len; i++)
        {
            Vector2 candidate;
            bool valid = false;
            int tryCnt = 0;

            do
            {
                candidate = RandomReachablePosition(origin, range);

                for (int j = 0; j < i; j++)
                {
                    if(Vector2.Distance(candidate, result[i]) < interval)
                    {
                        valid = true;
                        break;
                    }    
                }
                tryCnt++;
                if (tryCnt > 30)
                    break;
            } while (valid);
            result[i] = candidate;
        }
        return result;
    }

    private void LookAtTarget(bool lookAt, float rotSpeed)
    {
        if (lookAt)
        {
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            float targetAngle = GameMath.DirectionToAngle(direction);
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }
        else if (lookAt == false && navMesh.hasPath)
        {
            var path = navMesh.path;
            Vector2 pos = path.corners.Length >= 2 ? path.corners[1] : navMesh.destination;
            Vector2 direction = (pos - (Vector2)transform.position).normalized;
            float targetAngle = GameMath.DirectionToAngle(direction);
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }
    }

    public Collider2D GetNearestObs()
    {
        Vector2 origin = transform.position;
        return Physics2D.OverlapCircleAll(origin, recogDist, 1 << LayerMask.NameToLayer("Tile") | 1 << LayerMask.NameToLayer("ScanTile")).
            OrderBy(o => Vector2.SqrMagnitude((Vector2)o.transform.position - origin)).First();
    }

    public void Attack(Entity caster, float amount)
    {
        if (WeaponController.CanAttack)
        {
            WeaponController.UsingWeapon();
            OnEntityAttack(this, WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        }
    }

    protected bool HasAuditoryDetection()
    {
        return Util.RollChanceByPercent(listenPercentage.Evaluate(1f - Vector2.Distance(transform.position, playerTr.transform.position) / listenDist));
    }

    public abstract void OnEntityAttack(Entity caster, float amount);

    protected abstract void OnAuditoryDectected(Vector2 detectPos);

}
