using UnityEngine;

public class MeleeBasicAttack : IState<Enemy>
{
    public void Start(Enemy caster)
    {
        caster.Attack(caster, caster.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        caster.stateMachine.ChangeState(new MeleeEngagement(), 0.75f);
    }

    public void Update(Enemy caster)
    {
    }

    public void Finish(Enemy caster)
    {

    }
}
