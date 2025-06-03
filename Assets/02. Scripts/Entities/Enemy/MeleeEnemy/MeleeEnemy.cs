using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy, IAttackable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new Hammer());
        stateMachine = new StateMachine<Enemy>(this, new MeleePatrol());
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
}
