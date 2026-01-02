using UnityEngine;

public class Sniper : Enemy, IAttackable
{
    public override StateBase<Enemy> DefaultState => new SniperPatrol();

    public override StateBase<Enemy> AttackState => new SniperEngagement();

    [Range(0, 180f)] public float patrolRadius;
    [HideInInspector] public Vector2 originDir;
    [HideInInspector] public Vector2[] patrolAreaEdge = new Vector2[2];
    private Vector2 origin;

    protected override void Start()
    {
        origin = transform.position;
        float originAngle = GameMath.DirectionToAngle(transform.right.normalized);
        patrolAreaEdge = new Vector2[2] { origin + GameMath.AngleToDirection(originAngle - patrolRadius), origin + GameMath.AngleToDirection(originAngle + patrolRadius) };
        base.Start();
        Stat.SetDefault(DefaultStatType.MOVE_SPEED, 0);
        navMesh.isStopped = true;
        WeaponController.SetWeapon(new SniperRifle());
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(origin);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.cyan;
        Vector2 origin = transform.position;
        Gizmos.DrawLine(origin, origin + GameMath.AngleToDirection(GameMath.DirectionToAngle(transform.right.normalized) - patrolRadius).normalized * recogDist);
        Gizmos.DrawLine(origin, origin + GameMath.AngleToDirection(GameMath.DirectionToAngle(transform.right.normalized) + patrolRadius).normalized * recogDist);
    }
}
