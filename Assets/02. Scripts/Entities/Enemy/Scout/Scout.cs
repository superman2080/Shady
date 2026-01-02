using UnityEngine;

public class Scout : Enemy
{
    public override StateBase<Enemy> DefaultState { get => new ScoutPatrol(); }

    public override StateBase<Enemy> AttackState { get => new ScoutEngagement(); }

    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new EnemyCaller());
    }
}
