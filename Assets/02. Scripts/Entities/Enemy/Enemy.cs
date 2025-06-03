#nullable enable
#pragma warning disable CS8602
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class Enemy : Entity, IAttackable
{
    [HideInInspector] public StateMachine<Enemy>? stateMachine;
    [HideInInspector] public WeaponCtrl? weaponCtrl;
    [HideInInspector] public NavMeshAgent? navMesh;
    [HideInInspector] public Vector2 targetPos;

    [Header("Related to suspicion state")]
    public float searchTime;
    public float searchRange;
    public float engageTime;
    public bool isLookAtTarget { get; set; } = true;
    public float rotationSpeed = 150f;

    [Header("Related to sight")]
    public float recogDist;
    [Range(30, 90)] public float sightAngle;
    public LayerMask recogLayer;
    public Transform? playerTr { get; private set; }

    #region IAttackable
    public Coroutine AttackTimerCor { get; protected set; } = null;

    public WeaponCtrl WeaponController { get; protected set; }

    public WeaponStat weaponStat { get => WeaponController.weaponStat; }

    [SerializeField] public LayerMask AttackLayer { get => 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Player"); }
    #endregion

    protected override void Start()
    {
        base.Start();
        WeaponController = new WeaponCtrl(this);
        navMesh = gameObject.GetComponent<NavMeshAgent>();
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
        navMesh.angularSpeed = rotationSpeed;
        playerTr = FindAnyObjectByType<PlayerCtrl>().transform;
    }

    protected virtual void Update()
    {
        stateMachine?.Update();
        LookAtTarget(isLookAtTarget, rotationSpeed);
    }

    public bool IsPlayerInSight(float range, float angle, int layer)
    {
        List<GameObject>? list = FieldOfView(range, angle, layer);
        if (list == null)
            return false;
        return list.Exists(obj => obj.TryGetComponent(out PlayerCtrl player));
    }

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

    public void CallOtherEnemies(float range)
    {
        Vector2 origin = transform.position;
        Enemy[] enemies = Physics2D.OverlapCircleAll(origin, range, 1 << LayerMask.NameToLayer("Enemy")).Select(col => col.GetComponent<Enemy>()).ToArray();
        if (enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
            }
        }
    }

    private void LookAtTarget(bool lookAt, float rotSpeed)
    {
        if (lookAt)
        {
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }
    }

    public Collider2D GetNearestObs()
    {
        Vector2 origin = transform.position;
        return Physics2D.OverlapCircleAll(origin, searchRange, 1 << LayerMask.NameToLayer("Tile") | 1 << LayerMask.NameToLayer("ScanTile")).
            OrderBy(o => Vector2.SqrMagnitude((Vector2)o.transform.position - origin)).First();
    }

    public void Attack(Entity caster, float amount)
    {
        if (AttackTimerCor == null && WeaponController.nowWeapon != null)
        {
            WeaponController.UsingWeapon();
            OnEntityAttack(this, WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
            AttackTimerCor = StartCoroutine(AttackTimer(weaponStat.Get(WeaponStatType.ATTACK_SPEED)));
        }
    }


    private IEnumerator AttackTimer(float attackSpeed)
    {
        yield return new WaitForSeconds(1f / attackSpeed);
        AttackTimerCor = null;
    }

    public abstract void OnEntityAttack(Entity caster, float amount);
}
