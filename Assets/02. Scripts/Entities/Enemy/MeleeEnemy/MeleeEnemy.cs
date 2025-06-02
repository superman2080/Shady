using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine<Enemy>(this, new MeleePatrol());
        weaponCtrl = new WeaponCtrl(this, new Dagger());
        stat.SetDefault(StatType.MOVE_SPEED, 3);
        navMesh.speed = stat.Get(StatType.MOVE_SPEED);
        navMesh.angularSpeed = rotationSpeed;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEntityDied(Entity caster)
    {
    }

    protected override void OnTakeDamage(Entity caster, float amount)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnEntityAttack(Entity caster, float amount)
    {
    }
}
