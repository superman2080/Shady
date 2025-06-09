using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy, IAttackable
{
    public Vector3 originPos;

    public override IState<Enemy> DefaultState { get => new MeleePatrol();}
    public override IState<Enemy> AttackState { get => new MeleeEngagement();}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new Hammer());
        originPos = transform.position;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEntityDied(IAttackable caster)
    {
    }

    protected override void OnTakeDamage(IAttackable caster, float amount)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    public override void OnEntityAttack(Entity caster, float amount)
    {
    }

    protected override void OnAuditoryDectected(Vector2 detectPos)
    {
    }
}
