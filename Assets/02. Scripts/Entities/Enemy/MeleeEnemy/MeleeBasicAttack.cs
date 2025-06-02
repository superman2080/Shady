using UnityEngine;

public class MeleeBasicAttack : IState<Enemy>
{
    private float curHP;

    public float TransitionTime { get; set; } = 0.5f;

    public void Start(Enemy caster)
    {
        curHP = caster.HP;
    }

    public void Update(Enemy caster)
    {
        //caster.navMesh.destination = caster.player.transform.position;
        //if(caster.IsPlayerInSight(caster.stat.Get(StatType.ATTACK_DISTANCE), caster.weaponCtrl.)
    }

    public void Finish(Enemy caster)
    {

    }
}
