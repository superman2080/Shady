using UnityEngine;
using System.Linq;

public class SniperEngagement : IState<Enemy>
{
    private Timer engageTimer;
    private Timer aimingTimer;
    private RaycastHit2D[] originTargets;
    private Vector2 origin;
    private LineRenderer aimTrail;
    public void Start(Enemy caster)
    {
        caster.isLookAtTarget = true;

        caster.rotationSpeed = 30f;
        origin = caster.transform.position;
        originTargets = Physics2D.RaycastAll(origin, (Vector2)(caster.playerTr.position) - origin, caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE),
            1 << LayerMask.NameToLayer("Light") | 1 << LayerMask.NameToLayer("Shadow"));
        engageTimer = new Timer(10, () => { caster.stateMachine.ChangeState(caster.DefaultState, 1.5f); }, null, false);
        aimingTimer = new Timer(1, () => 
        { 
            caster.Attack(caster, caster.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
            aimingTimer.Reset();
        }, null, false);

        aimTrail = caster.GetComponent<LineRenderer>();
        aimTrail.enabled = true;
        aimTrail.SetPosition(0, origin);
    }

    public void Update(Enemy caster)
    {
        caster.targetPos = caster.playerTr.position;
        Vector2 targetDir = ((Vector2)(caster.playerTr.position) - origin).normalized;

        RaycastHit2D playerHit = Physics2D.Raycast(origin, caster.transform.right, caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE),
            1 << LayerMask.NameToLayer("Player"));
        if (playerHit)
        {
            RaycastHit2D[] curTargets = Physics2D.RaycastAll(origin, targetDir, caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE),
            1 << LayerMask.NameToLayer("Light") | 1 << LayerMask.NameToLayer("Shadow"));
            var except = curTargets.Except(originTargets).ToArray();

            // Retracking player
            if(curTargets.Length > 0 && curTargets.Length > originTargets.Length && except.Length > 0)
            {
                caster.stateMachine.ChangeState(new SniperEngagement(), 3f);
            }
            else
            {
                aimTrail.SetPosition(1, playerHit.point);
                aimingTimer.Update(Time.deltaTime);
            }
        }
        else
        {
            aimTrail.SetPosition(1, origin + (Vector2)caster.transform.right * caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE));
            engageTimer.Update(Time.deltaTime);
        }

        ////Target without Sight
        //if(GameMath.IsLookingDir(caster.transform, targetDir, threshold) == false)
        //{
        //    engageTimer.Update(Time.deltaTime);
        //    aimTrail.SetPosition(1, origin + targetDir * caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE));
        //}
        ////Target in sight
        //else
        //{
        //    RaycastHit2D[] curTargets = Physics2D.RaycastAll(origin, targetDir, caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE),
        //    1 << LayerMask.NameToLayer("Light") | 1 << LayerMask.NameToLayer("Shadow"));
        //    var except = curTargets.Except(originTargets).ToArray();
        //    foreach (var item in curTargets)
        //    {
        //        Debug.LogError(item.transform.name);
        //    }
        //    if (except.Length > 0)
        //    {
        //        Debug.LogError(except.First().transform.gameObject.name);
        //        engageTimer.Update(Time.deltaTime);
        //        aimTrail.SetPosition(1, except.First().point);
        //    }
        //    else
        //    {
        //        RaycastHit2D hit = Physics2D.Raycast(origin, targetDir, caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE),
        //             1 << LayerMask.NameToLayer("Player"));
        //        if (hit)
        //        {
        //            aimingTimer.Start();
        //            aimTrail.SetPosition(1, hit.point);
        //            aimingTimer.Update(Time.deltaTime);
        //        }
        //        //else
        //        //{
        //        //    aimTrail.SetPosition(1, origin + targetDir * caster.WeaponController.weaponStat.Get(WeaponStatType.ATTACK_DISTANCE));
        //        //}
        //    }
        //}

    }
    public void Finish(Enemy caster)
    {
        aimTrail.enabled = false;
    }

    private void Reload(Enemy caster)
    {
        aimingTimer = new Timer(1, () =>
        {
            caster.Attack(caster, caster.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        });
    }
}
