using UnityEngine;

public class MeleeBasicAttack : IState<Enemy>
{
    public void Enter(Enemy caster)
    {
        caster.Attack(caster, caster.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        caster.stateMachine.ChangeState(new MeleeEngagement(), 0.75f);
    }

    public void Execute(Enemy caster)
    {
    }

    public void Exit(Enemy caster)
    {

    }
}
