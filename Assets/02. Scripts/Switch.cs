using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerNameSpace;

public class Switch : MonoBehaviour, IInteractable
{
    public List<ISwitchable> clients = new List<ISwitchable>();
    public bool IsActivated { get; private set; } = false;

    public void HasTouched(Player player)
    {
        Toggle();
    }

    public void Toggle()
    {
        if (clients.Count <= 0)
        {
            Debug.LogError("Null");
            return;
        }

        for (int i = 0; i < clients.Count; i++)
        {
            if (IsActivated)
                clients[i].Activate();
            else
                clients[i].Deactivate();
        }

        IsActivated = !IsActivated;
    }
}
