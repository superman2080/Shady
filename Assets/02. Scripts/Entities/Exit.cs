using UnityEngine;

public class Exit : MonoBehaviour, ITouchable
{

    public void HasTouched(PlayerCtrl player)
    {
        var entities = FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (var entity in entities)
        {
            entity.canBehavior = false;
        }
        InGameUI.Instance.Fade(false, Color.black, 0.5f, 1f);
    }
}
