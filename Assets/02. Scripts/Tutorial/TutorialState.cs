using UnityEngine;

public abstract class TutorialState : MonoBehaviour, IState<TutorialController>
{
    public abstract void Enter(TutorialController caster);

    public abstract void Execute(TutorialController caster);

    public abstract void Exit(TutorialController caster);
}
