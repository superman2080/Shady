using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TutorialCameraFocusState : TutorialState
{
    public float focusTime;
    public Transform targetTr;
    private List<Transform> originTr;

    public override void Enter(TutorialController caster)
    {
        originTr = MainCineCam.Instance.targetTrList.ToList();
        MainCineCam.Instance.targetTrList.Clear();
        MainCineCam.Instance.targetTrList.Add(targetTr);
        Timer timer = new Timer(focusTime, () => caster.SetNextTutorial());
    }

    public override void Execute(TutorialController caster)
    {
    }

    public override void Exit(TutorialController caster)
    {
        MainCineCam.Instance.targetTrList.Remove(targetTr);
        MainCineCam.Instance.targetTrList.AddRange(originTr);
    }
}
