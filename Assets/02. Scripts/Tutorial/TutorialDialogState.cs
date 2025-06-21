using UnityEngine;
using System.Collections.Generic;

public class TutorialDialogState : TutorialState
{
    public List<DialogData> datas;
    private bool finishDialog = false;
    private Entity[] entities;

    public override void Enter(TutorialController caster)
    {
        entities = FindObjectsByType<Entity>(FindObjectsSortMode.InstanceID);
        foreach (var entity in entities)
        {
            entity.canBehavior = false;
        }
        InGameUI.Instance.dialogSystem.SetDialog(datas, () => {
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
        foreach (var entity in entities)
        {
            entity.canBehavior = true;
        }
    }
}
