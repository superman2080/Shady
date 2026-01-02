using UnityEngine;

public class MeleeBasicAttack : StateBase<Enemy>
{
    public override void Enter(Enemy caster)
    {
        caster.Attack(caster, caster.WeaponStat.Get(WeaponStatType.DAMAGE));
        caster.stateMachine.ChangeState(new MeleeEngagement());
    }

    public override void Execute(Enemy caster)
    {
    }

    public override void Exit(Enemy caster)
    {

    }
}
