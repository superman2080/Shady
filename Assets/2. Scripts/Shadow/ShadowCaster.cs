#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ShadowCaster : MonoBehaviour
{
    public float lightScale = 15f;
    public float minShadowScale = 5f;

    private List<Shadow> shadowList = new List<Shadow>();

    void Update()
    {
        GenerateShadow(lightScale, 1 << LayerMask.NameToLayer("Tile"));
    }

    private void GenerateShadow(float lS, int layer)
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, lS, layer);

        for (int i = 0; i < obstacles.Length; i++)
        {
            Shadow? shadow = null;
            if (shadowList.Count < obstacles.Length)
            {
                shadow = ShadowPool.Instance.InstantiateShadow(this);
                shadowList.Add(shadow);
            }
            else
            {
                shadow = shadowList[i];
            }
            List<Vector3>? objectVertices = GetColliderVertices(obstacles[i], lS, minShadowScale);
            List<Vector3>? shadowVertices = GetShadowVertices(obstacles[i].gameObject, objectVertices, lS, minShadowScale);
            shadow.GenerateShadow(shadowVertices);
        }

        if (shadowList.Count > obstacles.Length)
        {
            int removeCount = shadowList.Count - obstacles.Length; // 삭제할 개수 저장

            for (int i = shadowList.Count - 1; i >= Mathf.Max(0, shadowList.Count - removeCount); i--)
            {
                shadowList[i].gameObject.SetActive(false);
                shadowList.RemoveAt(i);
            }
        }
        shadowList = shadowList.OrderBy(obj => obj.GetInstanceID()).ToList();
    }

    private List<Vector3> GetColliderVertices(Collider2D collider, float shadowDist, float minShadowDist)
    {
        List<Vector3> vertices = new List<Vector3>();

        if (collider is PolygonCollider2D polyCollider)
        {
            foreach (Vector2 localPoint in polyCollider.points)
            {
                vertices.Add(collider.transform.TransformPoint(localPoint));
            }
        }
        else if (collider is BoxCollider2D boxCollider)
        {
            Vector2 size = boxCollider.size * 0.5f;
            Vector2[] localPoints = new Vector2[]
            {
                new Vector2(-size.x, -size.y),
                new Vector2(-size.x, size.y),
                new Vector2(size.x, size.y),
                new Vector2(size.x, -size.y)
            };

            foreach (Vector2 localPoint in localPoints)
            {
                vertices.Add(boxCollider.transform.TransformPoint(localPoint));
            }
        }
        return vertices;

    }

    private List<Vector3> GetShadowVertices(GameObject obs, List<Vector3> points, float lS, float mD)
    {
        obs.layer = LayerMask.NameToLayer("ScanTile");
        List<Vector3> result = new ();

        Vector3 origin = transform.position;
        foreach (var point in points)
        {

            Vector3 dir = (point - origin).normalized;
            


            float offset = 0.03f;
            Debug.DrawRay(origin, dir * lS, Color.blue);

            RaycastHit2D hit = Physics2D.CircleCast(origin, 0.01f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));
            if (hit && Vector3.Distance(point, hit.point) <= offset)
            {
                result.Add(hit.point);
                result.Add((Vector3)hit.point + dir * mD);
                Debug.DrawLine(origin, hit.point, Color.red);
            }
        }

        obs.layer = LayerMask.NameToLayer("Tile");



        var closePoints = result.OrderByDescending(p => Vector3.Distance(p, origin)).Take(result.Count / 2);
        Vector3 center = new Vector3(closePoints.Average(p => p.x), closePoints.Average(p => p.y));
        //Vector3 center = new Vector3(
        //    (result.Min(p => p.x) + result.Max(p => p.x)) / 2f,
        //    (result.Min(p => p.y) + result.Max(p => p.y)) / 2f
        //    );
        Debug.DrawLine(center + Vector3.left * 0.03f, center + Vector3.right * 0.03f, Color.blue, 0.1f);
        result = result.OrderByDescending(p => Mathf.Atan2(p.y - center.y, p.x - center.x)).ToList();
        if (result.Count > 0)
            result.Sort((a, b) =>
            {
                float angleA = Mathf.Atan2(a.y - origin.y, a.x - origin.x);
                float angleB = Mathf.Atan2(b.y - origin.y, b.x - origin.x);
                return angleA.CompareTo(angleB);
            });
        return result;
    }
}
