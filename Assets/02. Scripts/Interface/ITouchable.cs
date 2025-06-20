using UnityEngine;

public interface ITouchable<T> where T : MonoBehaviour
{
    void HasTouched(PlayerCtrl player);
}
