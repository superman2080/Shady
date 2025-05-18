using UnityEngine;

public class CSingleLookAt : IState<MainCineCam>
{
    public void Start(MainCineCam caster)
    {
        caster.SetOrthoSize(caster.minOrthoSize);
    }

    public void Update(MainCineCam caster)
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
            caster.stateMachine.ChangeState(new CMultipleLookAt());
        }
    }

    public void Finish(MainCineCam caster)
    {
    }
}
