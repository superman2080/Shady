using UnityEngine;

public class MeleeDash : IState<Enemy>
{
    private Vector2 targetPos;

    private Vector2 origin;
    private float elapsedTime;

    public void Start(Enemy caster)
    {
        origin = caster.transform.position;
        Vector2 playerPos = caster.playerTr.position;
        caster.navMesh.isStopped = true;
        caster.navMesh.ResetPath();
        caster.spriteRenderer.color = Color.blue;
        caster.rotationSpeed = 720f;

        targetPos = GameMath.GetOffsetPosition(origin, playerPos, 2);
    }

    public void Update(Enemy caster)
    {
        elapsedTime += Time.deltaTime;
        caster.transform.position = Vector2.Lerp(origin, targetPos, GetEaseOutT(elapsedTime, 0.25f));
        if (elapsedTime > 0.25f)
            caster.stateMachine.ChangeState(new MeleeEngagement(), 0.25f);
    }

    public void Finish(Enemy caster)
    {
        caster.isLookAtTarget = true;
        caster.navMesh.isStopped = false;
        (caster as Melee).SetDashTimer();
        caster.rotationSpeed = 150f;
    }

    private float GetEaseOutT(float elapsedTime, float duration)
    {
        float t = Mathf.Clamp01(elapsedTime / duration);         // 0~1로 정규화
        return 1f - Mathf.Pow(1f - t, 2f);                        // Ease-Out Quadratic
    }
}
