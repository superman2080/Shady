using UnityEngine;

public class CSingleLookAt : IState<MainCineCam>
{
    public float TransitionTime { get; set; }

    public void Enter(MainCineCam caster)
    {
        caster.SetOrthoSize(caster.minOrthoSize);
    }

    public void Execute(MainCineCam caster)
    {
        if(caster.targetTrList.Count <= 0)
        {
            caster.LookAt(Vector2.zero);
        }
        else if(caster.targetTrList.Count <= 1)
        {
            caster.LookAt(caster.targetTrList[0].position);
        }
        else
        {
            caster.stateMachine.ChangeStateImmediately(new CMultipleLookAt());
        }
    }

    public void Exit(MainCineCam caster)
    {
    }
}
