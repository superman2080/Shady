#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadowCaster : MonoBehaviour
{
    public float shadowDistance = 10f;      // maximum shadow length

    private List<Shadow> shadowList = new List<Shadow>();
    private float camWidth;

    void Start()
    {
        camWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
    }

    void Update()
    {
        GenerateShadow(shadowDistance, 1 << LayerMask.NameToLayer("Tile"));
    }

    private void GenerateShadow(float shadowDist, int layer)
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, camWidth, layer);

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

            List<Vector3>? objectVertices = GetColliderVertices(obstacles[i], shadowDist);
            //List<Vector3> points = new List<Vector3>();
            //foreach (var vertex in objectVertices)
            //{
            //    Vector3 direction = (vertex - transform.position).normalized;
            //    Vector3 shadowPoint = vertex + direction * shadowDist;
            //    points.Add(transform.InverseTransformPoint(vertex + transform.position));            // obstacle edges
            //    points.Add(transform.InverseTransformPoint(shadowPoint + transform.position));       // shadow end point
            //}
            //shadow.GenerateShadow(points);
            shadow.GenerateShadow(objectVertices);
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

    private List<Vector3> GetColliderVertices(Collider2D collider, float shadowDist)
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

        // 1. 각도 및 점 저장
        List<(float angle, Vector3 point)> anglePoints = new ();

        foreach (var point in vertices)
        {
            Vector2 dir = (point - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            anglePoints.Add((angle, point));
        }

        // 2. 각도로 정렬
        anglePoints.Sort((a, b) => a.angle.CompareTo(b.angle));

        // 3. 가장 큰 각도 차이가 나는 두 점 찾기
        float maxGap = 0;
        int maxIndex = 0;

        for (int i = 0; i < anglePoints.Count; i++)
        {
            int nextIndex = (i + 1) % anglePoints.Count;

            float currentAngle = anglePoints[i].angle;
            float nextAngle = anglePoints[nextIndex].angle;

            float gap = (nextAngle - currentAngle + 360f) % 360f;

            if (gap > maxGap)
            {
                maxGap = gap;
                maxIndex = nextIndex;
            }
        }

        // 4. 그림자 정점 만들기
        Vector3 pointA = anglePoints[maxIndex % anglePoints.Count].point;
        Vector3 pointB = anglePoints[(maxIndex - 1 + anglePoints.Count) % anglePoints.Count].point;

        List<Vector3> result = new List<Vector3>();
        result.Add(pointA);
        result.Add(pointA + (pointA - transform.position).normalized * shadowDist);
        result.Add(pointB + (pointB - transform.position).normalized * shadowDist);
        result.Add(pointB);

        return result;
    }

}
