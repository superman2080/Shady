#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadowCaster : MonoBehaviour, ICameraLookable
{
    public float lightScale = 20f;                  // Light source scale (Sense distance is lightScale - minShadowScale)
    public float minShadowScale = 5f;
    private List<Shadow> shadowList = new List<Shadow>();       // Shadow Object Pool
    private PlayerCtrl player;
    void OnEnable()
    {
        EnableCamera();
        player = FindAnyObjectByType<PlayerCtrl>();
    }

    void Update()
    {
        GenerateShadow(lightScale, minShadowScale, 1 << LayerMask.NameToLayer("Tile"));
        Vector2 origin = transform.position;

        if(Vector2.Distance(origin, player.transform.position) > lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == true)
        {
            DisableCamera();
        }
        else if (Vector2.Distance(origin, player.transform.position) < lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == false)
        {
            EnableCamera();
        }
    }

    void OnDisable()
    {
        DisableCamera();
    }

    private void GenerateShadow(float lS, float mS, int layer)
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, lS - mS, layer);

        // Object Pool (Generating shadow when collided obstacles)
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
            List<Vector3>? objectVertices = GetColliderVertices(obstacles[i], lS, mS);
            List<Vector3>? shadowVertices = GetShadowVertices(obstacles[i].gameObject, objectVertices, lS, mS);
            shadow.GenerateShadow(shadowVertices);
        }
        //

        // Delete when shadows outnumber obstacles
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
                vertices.Add(collider.transform.TransformPoint(polyCollider.points[i]));
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
        return vertices;
    }


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
        Vector2 minHit = Vector2.zero;
        Vector2 maxHit = Vector2.zero;

        foreach (var point in points)
        {
            Vector3 dir = (point - origin).normalized;

            float offset = 0.1f;

            RaycastHit2D hit = Physics2D.CircleCast(origin, 0.005f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));
            RaycastHit2D farHit = Physics2D.CircleCast(point + dir * offset, 0.003f, dir, lS, 1 << LayerMask.NameToLayer("ScanTile"));


            if (hit && Vector3.Distance(point, hit.point) <= offset)         // When cast inner point
            {
                innerPoints.Add(point);
                if (farHit && Vector3.Distance(point, farHit.point) > offset * 1.5f)        // 
                {
                    innerPoints.Add(farHit.point);
                }
                else if (!farHit)
                {
                    float shadowLen = lS - Vector2.Distance(origin, point) > mD ? lS - Vector2.Distance(origin, point) : mD;

                    outerPoints.Add(point + dir * shadowLen);
                    float eachAngle = Mathf.Atan2(point.y - origin.y, point.x - origin.x) * Mathf.Rad2Deg;      // Most largest angle in points
                    if (minAngle > eachAngle)
                    {
                        minAngle = eachAngle;
                        minDir = new Vector2(Mathf.Cos(minAngle * Mathf.Deg2Rad), Mathf.Sin(minAngle * Mathf.Deg2Rad));
                        minHit = point;
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
                float shadowLen = lS - Vector2.Distance(origin, point) > mD ? lS - Vector2.Distance(origin, point) : mD;
                outerPoints.Add(point + dir * shadowLen);
            }
        }

        Vector2 midDir = (minDir + maxDir).normalized;
        if (!IsFirstVertice(origin, innerPoints, maxHit, midDir) && !IsLastVertice(origin, innerPoints, maxHit, midDir) ||
            !IsFirstVertice(origin, innerPoints, minHit, midDir) && !IsLastVertice(origin, innerPoints, minHit, midDir)) 
            midDir *= -1;

        innerPoints = innerPoints.OrderByDescending(p => {
            float angle = GetSignedAngleRelativeTo(origin, p, midDir); // Subtract each angle by offset angle
            return angle;
        }).ToList();



        outerPoints = outerPoints.OrderBy(p => {  
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

    public void EnableCamera()
    {
        MainCineCam.Instance.targetTrList.Add(transform);
    }

    public void DisableCamera()
    {
        MainCineCam.Instance.targetTrList.Remove(transform);
    }
}
