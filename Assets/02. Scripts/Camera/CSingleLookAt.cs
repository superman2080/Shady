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
        else
        {
            caster.LookAt(caster.targetTrList[0].position);
        }
    }

    public void Exit(MainCineCam caster)
    {
    }
}
