using UnityEngine;

public class Sniper : Enemy, IAttackable
{
    public override IState<Enemy> DefaultState => new SniperPatrol();

    public override IState<Enemy> AttackState => throw new System.NotImplementedException();

    [Range(0, 180f)] public float patrolRadius;
    [HideInInspector] public Vector2 originDir;
    public Vector2[] patrolAreaEdge = new Vector2[2];
    private Vector2 origin;

    protected override void Start()
    {
        origin = transform.position;
        float originAngle = GameMath.DirectionToAngle(transform.right.normalized);
        patrolAreaEdge = new Vector2[2] { origin + GameMath.AngleToDirection(originAngle - patrolRadius), origin + GameMath.AngleToDirection(originAngle + patrolRadius) };
        base.Start();
        entityStat.SetDefault(EntityStatType.MOVE_SPEED, 0);
        navMesh.isStopped = true;
        WeaponController.SetWeapon(new SniperRifle());
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(origin);
    }


    public override void OnEntityAttack(Entity caster, float amount)
    {
    }

    protected override void OnAuditoryDectected(Vector2 detectPos)
    {
    }

    protected override void OnEntityDied(IAttackable caster)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnTakeDamage(IAttackable caster, float amount)
    {
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
