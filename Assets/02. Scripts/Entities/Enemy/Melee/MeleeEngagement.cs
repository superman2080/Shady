using UnityEngine;

public class MeleeEngagement : IState<Enemy>
{
    public float TransitionTime { get; set; } = 0.5f;
    private const float dashDist = 5;
    private Vector2 playerPos;
    private Vector2 origin;

    public void Start(Enemy caster)
    {
        caster.navMesh.isStopped = false;
        caster.isLookAtTarget = true;
        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 2f);
        caster.spriteRenderer.color = Color.red;
        caster.rotationSpeed = 150f;
    }

    public void Update(Enemy caster)
    {
        if (caster.playerTr == null)
            caster.stateMachine.ChangeStateImmediately(new MeleePatrol());

        origin = caster.transform.position;
        playerPos = caster.playerTr.transform.position;
        caster.targetPos = playerPos;
        caster.navMesh.destination = playerPos;

        if ((origin - playerPos).magnitude > dashDist && (caster as Melee).CanDash)
        {
            caster.stateMachine.ChangeState(new MeleeDash(), 0.25f);
        }

        if ((origin - playerPos).magnitude <= caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE) && caster.WeaponController.CanAttack)
        {
            caster.stateMachine.ChangeStateImmediately(new MeleeBasicAttack());
        }
    }

    public void Finish(Enemy caster)
    {
        caster.navMesh.isStopped = true;
        caster.isLookAtTarget = false;
        caster.navMesh.ResetPath();
    }
}
