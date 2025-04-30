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
    Vector2 midDir;
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
            for (int i = 0; i < polyCollider.points.Length; i++)
            {
                vertices.Add((collider.transform.TransformPoint(polyCollider.points[i])));
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

            for (int i = 0; i < localPoints.Length; i++)
            {
                vertices.Add((boxCollider.transform.TransformPoint(localPoints[i])));
            }
        }

        //Vector3 origin = transform.position; // 빛 위치
        //int outerPointIdx = vertices.Select((p, i) => new { Point = p, Index = i })
        //         .OrderBy(x => Vector3.SqrMagnitude(x.Point - origin))
        //         .First().Index;

        //Debug.Log($"outeridx: {outerPointIdx}");

        //vertices = vertices.OrderBy(p =>
        //{
        //    float angle = Mathf.Atan2(p.y - origin.y, p.x - origin.x) * Mathf.Rad2Deg;
        //    if (angle < 0) angle += 360f;
        //    return angle;
        //}).ToList();

        return vertices/*.Skip(outerPointIdx).Concat(vertices.Take(outerPointIdx)).ToList()*/;
    }

    //private List<Vector3> GetShadowVertices(GameObject obs, List<Vector3> points, float lS, float mD)
    //{
    //    obs.layer = LayerMask.NameToLayer("ScanTile");

    //    List<Vector3> reachablePoint = new ();
    //    List<Vector3 > firstPoint = new ();
    //    List<Vector3> farPoint = new ();
    //    Vector3 origin = transform.position;
    //    foreach (var point in points)
    //    {

    //        Vector3 dir = (point - origin).normalized;

    //        float offset = 0.1f;

    //        Debug.DrawRay(origin, dir * lS, Color.blue);

    //        RaycastHit2D hit = Physics2D.CircleCast(origin, 0.003f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));
    //        if (hit && Vector3.Distance(point, hit.point) <= offset)
    //        {
    //            float angle = Mathf.Atan2(point.y - origin.y, point.x - origin.x) * Mathf.Rad2Deg;
    //            reachablePoint.Add(hit.point);
    //            Debug.DrawLine(origin, hit.point, Color.red);
    //        }
    //    }

    //    Vector3 center = new Vector3(reachablePoint.Average(p => p.x), reachablePoint.Average(p => p.y));

    //    float offsetAngle = Mathf.Atan2(center.y - origin.y, center.x - origin.x) * Mathf.Rad2Deg;

    //    reachablePoint = reachablePoint.OrderBy(p =>
    //    {
    //        float angle = Mathf.Atan2(p.y - origin.y, p.x - origin.x) * Mathf.Rad2Deg;
    //        if (angle < 0) angle += 360f;
    //        Debug.Log($"{p}, {angle + offsetAngle}");
    //        return angle;
    //    }).ToList();

    //    foreach (var point in reachablePoint)
    //    {
    //        Vector3 dir = (point - origin).normalized;
    //        float offset = 0.1f;
    //        RaycastHit2D farHit = Physics2D.CircleCast(point + dir * offset, 0.003f, dir, lS - Vector3.Distance(origin, point), 1 << LayerMask.NameToLayer("ScanTile"));

    //        if (farHit && Vector3.Distance(point, farHit.point) > offset * 1.5f)
    //        {
    //            firstPoint.Add(point);
    //            farPoint.Add((Vector3)farHit.point + dir * mD);
    //            firstPoint.Add(farHit.point);
    //        }
    //        else
    //        {
    //            firstPoint.Add(point);
    //            farPoint.Add(point + dir * mD);
    //        }
    //    }

    //    RaycastHit2D farHit = Physics2D.CircleCast(point + dir * offset, 0.003f, dir, lS - Vector3.Distance(origin, point), 1 << LayerMask.NameToLayer("ScanTile"));

    //    if (farHit && Vector3.Distance(point, farHit.point) > offset * 1.5f)
    //    {
    //        firstPoint.Add(hit.point);
    //        firstPoint.Add(farHit.point);
    //        farPoint.Add((Vector3)farHit.point + dir * mD);
    //    }
    //    else
    //    {
    //        firstPoint.Add((hit.point));
    //        farPoint.Add((Vector3)hit.point + dir * mD);
    //    }

    //    obs.layer = LayerMask.NameToLayer("Tile");
    //    farPoint.Reverse();
    //    firstPoint.InsertRange(0, farPoint);

    //    return firstPoint;

    //}

    private List<Vector3> GetShadowVertices(GameObject obs, List<Vector3> points, float lS, float mD)
    {
        obs.layer = LayerMask.NameToLayer("ScanTile");

        List<Vector3> innerPoints = new ();
        List<Vector3> outerPoints = new ();


        Vector3 origin = transform.position;
        float minAngle = float.PositiveInfinity;
        float maxAngle = float.NegativeInfinity;
        Vector2 minDir = Vector2.zero;
        Vector2 maxDir = Vector2.zero;
        Vector2 maxHit = Vector2.zero;

        foreach (var point in points)
        {
            Vector3 dir = (point - origin).normalized;

            float offset = 0.1f;

            RaycastHit2D hit = Physics2D.CircleCast(origin, 0.005f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));
            RaycastHit2D farHit = Physics2D.CircleCast(point + dir * offset, 0.003f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));

            if(hit && Vector3.Distance(point, hit.point) <= offset)
            {
                innerPoints.Add(point);
                if (farHit && Vector3.Distance(point, farHit.point) > offset * 1.5f)
                {
                    innerPoints.Add(farHit.point);
                }
                else if (!farHit)
                {
                    outerPoints.Add(point + dir * mD);
                    float eachAngle = Mathf.Atan2(point.y - origin.y, point.x - origin.x) * Mathf.Rad2Deg;      // Most largest angle in points
                    if (minAngle > eachAngle)
                    {
                        minAngle = eachAngle;
                        minDir = new Vector2(Mathf.Cos(minAngle * Mathf.Deg2Rad), Mathf.Sin(minAngle * Mathf.Deg2Rad));
                    }
                    if (maxAngle < eachAngle)
                    {
                        maxAngle = eachAngle;
                        maxDir = new Vector2(Mathf.Cos(maxAngle * Mathf.Deg2Rad), Mathf.Sin(maxAngle * Mathf.Deg2Rad));
                        maxHit = point;
                    }
                }
            }
            else if(hit && Vector3.Distance(point, hit.point) > offset * 1.5f)
            {
                outerPoints.Add(point + dir * mD);
            }
        }

        Vector2 midDir = (minDir + maxDir).normalized;
        if (!IsFirstVertice(origin, innerPoints, maxHit, midDir) && !IsLastVertice(origin, innerPoints, maxHit, midDir)) 
            midDir *= -1;

        Debug.DrawRay(origin, midDir * lS, Color.blue, 0.02f);
        //int closestIdx = innerPoints.Select((p, i) => new { Idx = i, Dir = p - origin.normalized}).OrderBy(p => Vector2.Angle(p.Dir, midDir)).First().Idx;
        //Debug.Log($"{closestIdx}: {innerPoints[closestIdx]}");


        innerPoints = innerPoints.OrderByDescending(p => {
            float angle = GetSignedAngleRelativeTo(origin, p, midDir); // Subtract each angle by offset angle
            return angle;
        }).ToList();



        outerPoints = outerPoints.OrderBy(p => {
            //float angle = (Mathf.Atan2(p.y - origin.y, p.x - origin.x) * Mathf.Rad2Deg) + Mathf.Atan2(midDir.y, midDir.x) * Mathf.Rad2Deg;        
            float angle = GetSignedAngleRelativeTo(origin, p, midDir); // Subtract each angle by offset angle
            return angle;
        }).ToList();

        innerPoints.AddRange(outerPoints);
        obs.layer = LayerMask.NameToLayer("Tile");
        return innerPoints;
    }

    private float GetSignedAngleRelativeTo(Vector2 origin, Vector2 target, Vector2 basis)
    {
        Vector2 dir = (target - origin).normalized;
        float angle = Vector2.SignedAngle(basis.normalized, dir);
        return angle;
    }

    private bool IsFirstVertice(Vector3 origin, List<Vector3> vertices, Vector3 vertice, Vector2 basis)
    {
        float angle = GetSignedAngleRelativeTo(origin, vertice, basis);
        foreach (var point in vertices)
        {
            if (GetSignedAngleRelativeTo(origin, point, basis) > angle)
                return false;
        }
        return true;
    }

    private bool IsLastVertice(Vector3 origin, List<Vector3> vertices, Vector3 vertice, Vector2 basis)
    {
        float angle = GetSignedAngleRelativeTo(origin, vertice, basis);
        foreach (var point in vertices)
        {
            if (GetSignedAngleRelativeTo(origin, point, basis) < angle)
                return false;
        }
        return true;
    }
}
