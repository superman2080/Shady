using UnityEngine;

public class Scout : Enemy
{
    public override IState<Enemy> DefaultState { get => new ScoutPatrol(); }

    public override IState<Enemy> AttackState { get => new ScoutEngagement(); }

    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new EnemyCaller());
    }

    public override void OnEntityAttack(Entity caster, float amount)
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

    protected override void OnAuditoryDectected(Vector2 detectPos)
    {
        if (stateMachine.State?.GetType().Name == DefaultState.GetType().Name)
        {
            Vector2 origin = transform.position;
            Vector2 targetPos = GameMath.GetOffsetPosition(origin, detectPos, 2);
            stateMachine.ChangeState(new Doubt(targetPos), 0.5f);
        }
    }
}
