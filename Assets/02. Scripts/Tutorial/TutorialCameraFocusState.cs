using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TutorialCameraFocusState : TutorialState
{
    [Min(0.5f)] public float moveTime = 0.5f;
    [Min(0.5f)] public float focusTime = 0.5f;
    public Transform targetTr;
    private List<Transform> originTargetTr;

    public override void Enter(TutorialController caster)
    {
        originTargetTr = MainCineCam.Instance.targetTrList.ToList();
        MainCineCam.Instance.targetTrList.Clear();
        MainCineCam.Instance.targetTrList.Add(targetTr);

        StartCoroutine(MoveToCor(caster, transform.position, targetTr.position));
    }

    public override void Execute(TutorialController caster)
    {
    }

    public override void Exit(TutorialController caster)
    {
        MainCineCam.Instance.targetTrList.Remove(targetTr);
        MainCineCam.Instance.targetTrList.AddRange(originTargetTr);
    }

    private IEnumerator MoveToCor(TutorialController caster, Vector2 origin, Vector2 moveTo)
    {
        for (float eT = 0; eT < moveTime; eT+=Time.deltaTime)
        {
            targetTr.position = Vector3.Lerp(origin, moveTo, eT / moveTime);
            yield return null;
        }
        targetTr.position = moveTo;
        Timer timer = new Timer(focusTime, () => caster.SetNextTutorial());
    }
}
