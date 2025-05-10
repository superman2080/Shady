#nullable enable
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        var t = FieldOfView(10, 90, (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("ScanTile")) | (1 << LayerMask.NameToLayer("Shadow")));
        if(t != null)
        {
            foreach (var item in t)
            {
                Debug.Log(item.name);
                Debug.DrawRay(transform.position, (item.transform.position - transform.position).normalized * 10f, Color.red, 0.1f);
            }
        }
    }

    protected List<GameObject>? FieldOfView(float range, float angle, int layer)
    {
        Vector2 origin = transform.position;
        List<GameObject> result = new List<GameObject>();
        Collider2D[] targets = Physics2D.OverlapCircleAll(origin, range, layer);
        if (targets.Length <= 0)
            return null;
        else
        {
            foreach (var target in targets)
            {
                Vector2 targetPos = target.transform.position;
                Vector2 dir = (targetPos - origin).normalized;
                float theta = Mathf.Acos(Vector3.Dot(transform.right, dir)) * Mathf.Rad2Deg;

                if(Physics2D.Raycast(origin, dir, range, layer).collider == target && theta <= angle)
                {
                    result.Add(target.gameObject);
                }
            }
        }
        return result;
    }

    protected override void OnEntityDied(Entity caster)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnTakeDamage(Entity caster, float amount)
    {
    }
}
