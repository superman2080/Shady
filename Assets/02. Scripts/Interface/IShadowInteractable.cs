using UnityEngine;

public interface IShadowInteractable
{
    public void OnEnter(Collider2D other);
    public void OnStay(Collider2D other);
    public void OnExit(Collider2D other);
}
