using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PolygonCollider2D))]
public class Shadow : MonoBehaviour
{
    public ShadowCaster lightSource;
    public PolygonCollider2D col { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = gameObject.GetComponent<PolygonCollider2D>();
        col.pathCount = 1;
        Debug.Log(col.name);
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
    }

    public void SetCollision(List<Vector3> points)
    {
        Vector3 center = new Vector3(points.Average(p => p.x), points.Average(p => p.y));

        var sorted = points.OrderBy(p => Mathf.Atan2(p.y - center.y, p.x - center.x)).Select(v => new Vector2(v.x, v.y)).ToList();
        col.SetPath(0, sorted);
    }

    private void OnDisable()
    {
        lightSource = null;
    }
}
