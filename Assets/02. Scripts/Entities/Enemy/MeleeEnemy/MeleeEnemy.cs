using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy, IAttackable
{
    [HideInInspector] public Vector3 originPos;

    public override IState<Enemy> DefaultState { get => new MeleePatrol();}
    public override IState<Enemy> AttackState { get => new MeleeEngagement();}

    [Min(0.5f)] public float dashCoolTime;
    public bool CanDash => canDash;
    private bool canDash;
    private Timer dashTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new Hammer());
        originPos = transform.position;
        dashTimer = new Timer(dashCoolTime, () => { canDash = true; });
    }

    protected override void Update()
    {
        base.Update();
        dashTimer.Update(Time.deltaTime);
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

    public void SetDashTimer()
    {
        dashTimer.Reset(() => { canDash = false; });
    }
}
