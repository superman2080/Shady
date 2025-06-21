using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        Screen.SetResolution(1920, 1080, false);
    }
}
