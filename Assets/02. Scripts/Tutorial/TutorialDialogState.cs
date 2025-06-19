using UnityEngine;
using System.Collections.Generic;

public class TutorialDialogState : TutorialState
{
    public List<DialogData> datas;
    private bool finishDialog = false;

    public override void Enter(TutorialController caster)
    {
        UI.Instance.SetDialog(datas, () => {
            finishDialog = true;
        });
    }

    public override void Execute(TutorialController caster)
    {
        if (finishDialog == true)
            caster.SetNextTutorial();
    }

    public override void Exit(TutorialController caster)
    {
    }
}
