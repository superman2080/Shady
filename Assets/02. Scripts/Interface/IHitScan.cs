using UnityEngine;

public interface IHitScan
{
    LineRenderer Afterimage { get; set; }

    void Fire(IAttackable user, Vector2 origin, Vector2 dir);

    void OnHit(Collider2D other);

}
