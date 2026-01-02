using UnityEngine;
using PlayerNameSpace;

public class Exit : MonoBehaviour, IInteractable
{

    public void HasTouched(Player player)
    {
        var entities = FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (var entity in entities)
        {
            entity.canBehavior = false;
        }
        InGameUI.Instance.Fade(false, Color.black, 0.5f, 1f);
    }
}
