using UnityEngine;
using System.Collections;

public class Melee : Enemy
{
    [HideInInspector] public Vector3 originPos;

    public override StateBase<Enemy> DefaultState { get => new MeleePatrol();}
    public override StateBase<Enemy> AttackState { get => new MeleeEngagement();}

    [Min(0.5f)] public float dashCoolTime;
    public bool CanDash => canDash;


    private bool canDash = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        WeaponController.SetWeapon(new Hammer());
        originPos = transform.position;
    }


    //protected override void OnTakeDamage(IAttackable caster, float amount)
    //{
    //    if (stateMachine.CompareState(stateMachine.CurState, DefaultState) ||
    //        stateMachine.CompareState(stateMachine.CurState, "Doubt"))
    //        stateMachine.ChangeStateImmediately(AttackState);
    //}

    //protected override void OnAuditoryDectected(Vector2 detectPos)
    //{
    //    if(stateMachine.CompareState(stateMachine.CurState, DefaultState))
    //    {
    //        Vector2 origin = transform.position;
    //        Vector2 targetPos = GameMath.GetOffsetPosition(origin, detectPos, 2);
    //        stateMachine.ChangeState(new Doubt(targetPos), 0.5f);
    //    }
    //}

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    public void SetDashTimer()
    {
        canDash = false;
        new Timer(dashCoolTime, () => { canDash = true; });
    }
}
