using UnityEngine;

public interface ISwitchable
{
    public bool IsActivated { get; }
    public void Activate();
    public void Deactivate();
}
