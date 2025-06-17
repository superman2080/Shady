using System.Collections;
using UnityEngine;

public class SniperRifleProjectile : MonoBehaviour, IHitScan
{
    public LineRenderer Afterimage { get; set; }
    private float dist;
    void Awake()
    {
        Afterimage = gameObject.GetComponent<LineRenderer>();
        dist = Camera.main.orthographicSize * 2f * Camera.main.aspect;
    }

    public void Fire(IAttackable user, Vector2 origin, Vector2 dir)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, dist, user.AttackLayer);
        foreach (var hit in hits)
        {
            hit.transform.GetComponent<IDamagable>().TakeDamage(user, user.WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
            OnHit(hit.collider);
        }
        StartCoroutine(AfterimageEffectCor(user, origin, dir, 0.2f, 0.2f));
    }

    public void OnHit(Collider2D other)
    {

    }

    private IEnumerator AfterimageEffectCor(IAttackable user, Vector2 origin, Vector2 dir, float width, float time)
    {
        Afterimage.SetPositions(new Vector3[] { origin, origin + dir * dist });
        Afterimage.startWidth = Afterimage.endWidth = width;
        for (float eT = 0; eT < time; eT+=Time.deltaTime)
        {
            Afterimage.startWidth = Afterimage.endWidth = Mathf.Lerp(width, 0f, eT / time);
            yield return null;
        }
        Afterimage.startWidth = Afterimage.endWidth = 0;
        Destroy(gameObject);
    }
}
