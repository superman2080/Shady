using UnityEngine;

public class MeleeEngagement : IState<Enemy>
{
    public float TransitionTime { get; set; } = 0.5f;
    private const float dashDist = 10;
    private Vector2 playerPos;
    private Vector2 origin;

    public void Start(Enemy caster)
    {
        caster.navMesh.isStopped = false;
        caster.isLookAtTarget = true;


        caster.entityStat.SetDefault(EntityStatType.MOVE_SPEED, 1.5f);
        caster.spriteRenderer.color = Color.red;
    }

    public void Update(Enemy caster)
    {
        if (caster.navMesh.isStopped == false)
            caster.navMesh.destination = playerPos;

        origin = caster.transform.position;
        playerPos = caster.playerTr.transform.position;
        caster.targetPos = playerPos;

        if ((origin - playerPos).magnitude > dashDist)
        {
            caster.stateMachine.ChangeState(new MeleeDash(), 0.25f);
        }
    }

    public void Finish(Enemy caster)
    {
        caster.isLookAtTarget = false;
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
    }
}
