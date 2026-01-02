using UnityEngine;

public class CSingleLookAt : StateBase<MainCineCam>
{
    public float TransitionTime { get; set; }

    public override void Enter(MainCineCam caster)
    {
        caster.SetOrthoSize(caster.minOrthoSize);
    }

    public override void Execute(MainCineCam caster)
    {
        if(caster.targetTrList.Count <= 0)
        {
            caster.LookAt(Vector2.zero);
        }
        else
        {
            caster.LookAt(caster.targetTrList[0].position);
        }
    }

    public override void Exit(MainCineCam caster)
    {
    }
}
