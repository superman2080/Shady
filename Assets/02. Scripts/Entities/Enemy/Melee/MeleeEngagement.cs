using UnityEngine;

public class MeleeEngagement : StateBase<Enemy>
{
    public float TransitionTime { get; set; } = 0.5f;
    private const float dashDist = 5;
    private Vector2 playerPos;
    private Vector2 origin;

    public override void Enter(Enemy caster)
    {
        caster.navMesh.isStopped = false;
        caster.isLookAtTarget = true;
        caster.Stat.SetDefault(DefaultStatType.MOVE_SPEED, 2f);
        caster.spriteRenderer.color = Color.red;
        caster.rotationSpeed = 540f;
    }

    public override void Execute(Enemy caster)
    {
        if (caster.playerTr == null)
            caster.stateMachine.ChangeState(new MeleePatrol());

        origin = caster.transform.position;
        playerPos = caster.playerTr.transform.position;
        caster.targetPos = playerPos;
        caster.navMesh.destination = playerPos;

        if ((origin - playerPos).magnitude > dashDist && (caster as Melee).CanDash)
        {
            caster.stateMachine.ChangeState(new MeleeDash());
        }

        if ((origin - playerPos).magnitude <= caster.WeaponStat.Get(WeaponStatType.ATTACK_DISTANCE) && caster.WeaponController.CanAttack)
        {
            caster.stateMachine.ChangeState(new MeleeBasicAttack());
        }
    }

    public override void Exit(Enemy caster)
    {
        caster.navMesh.isStopped = true;
        caster.isLookAtTarget = false;
        caster.navMesh.ResetPath();
    }
}
