using UnityEngine;
using UnityEngine.UI;

public class TutorialMoveTo : MonoStateBase<TutorialController>
{
    public Transform targetTr;
    public Transform destinationTr;
    public GameObject arrowPrefab;
    private RectTransform arrow;

    public override void Enter(TutorialController caster)
    {
        arrow = Instantiate(arrowPrefab, InGameUI.Instance.transform).GetComponent<RectTransform>();
    }

    public override void Execute(TutorialController caster)
    {
        arrow.position = Camera.main.WorldToScreenPoint(destinationTr.transform.position);
        if ((destinationTr.position - targetTr.position).sqrMagnitude <= 1f)
            caster.SetNextTutorial();
    }

    public override void Exit(TutorialController caster)
    {
        Destroy(arrow.gameObject);
    }
}
