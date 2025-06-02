#nullable enable
#pragma warning disable CS8618
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class ShadowCaster : MonoBehaviour, ICameraLookable
{
    [Header("Related to UI")]
    [Range(1f, 100f)] public float maintainTime;
    public Slider sliderUI;

    [Header("Light Attribute")]
    public float lightScale = 20f;                  // Light source scale (Sense distance is lightScale - minShadowScale)
    public float minShadowScale = 5f;
    private Light2D light2D;
    private List<Shadow> shadowList = new List<Shadow>();       // Shadow Object Pool
    private PlayerCtrl player;


    private Collider2D[] curObstacles;
    private Collider2D[] lateObstacles;
    private List<List<Vector3>> shadowVertices = new List<List<Vector3>>();
    private Vector3 latePos;


    void OnEnable()
    {
        EnableCamera();
        player = FindAnyObjectByType<PlayerCtrl>();
        sliderUI.value = 1;
        StartCoroutine(DecreaseStamina(maintainTime));
    }

    void Start()
    {
        light2D = gameObject.GetComponent<Light2D>();
    }

    void Update()
    {
        Vector2 origin = transform.position;

        if(Vector2.Distance(origin, player.transform.position) > lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == true)
        {
            DisableCamera();
        }
        else if (Vector2.Distance(origin, player.transform.position) < lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == false)
        {
            EnableCamera();
        }

        GenerateShadow(lightScale, minShadowScale, 1 << LayerMask.NameToLayer("Tile"));
        light2D.intensity = lightScale;
    }

    void LateUpdate()
    {
        lateObstacles = curObstacles;
        latePos = transform.position;
    }

    void OnDisable()
    {
        foreach (var shadow in shadowList)
        {
            if (shadow != null)
            {
                shadow.gameObject.SetActive(false);
            }
        }
        shadowList.Clear();
        DisableCamera();
    }

    private void GenerateShadow(float lS, float mS, int layer)
    {
        curObstacles = Physics2D.OverlapCircleAll(transform.position, lS - mS, layer);


        if (transform.position.Equals(latePos) 
            && curObstacles.Length == lateObstacles.Length 
            && curObstacles.All(o => lateObstacles.Any(l => o.transform.position.x == l.transform.position.x && o.transform.position.y == l.transform.position.y)))
        {
            for (int i = 0; i < curObstacles.Length; i++)
            {
                shadowList[i].GenerateShadow(shadowVertices[i]);
            }


        }
        else
        {
            shadowVertices.Clear();
            // Object Pool (Generating shadow when collided obstacles)
            for (int i = 0; i < curObstacles.Length; i++)
            {
                Shadow? shadow = null;
                if (shadowList.Count < curObstacles.Length)
                {
                    shadow = ShadowPool.Instance.InstantiateShadow(this);
                    shadowList.Add(shadow);
                }
                else
                {
                    shadow = shadowList[i];
                }
                List<Vector3>? objectVertices = GetColliderVertices(curObstacles[i], lS, mS);
                shadowVertices.Add(GetShadowVertices(curObstacles[i].gameObject, objectVertices, lS, mS));
                shadow.GenerateShadow(shadowVertices[i]);
            }
            //

            // Delete when shadows outnumber obstacles
            if (shadowList.Count > curObstacles.Length)
            {
                int removeCount = shadowList.Count - curObstacles.Length; // 삭제할 개수 저장

                for (int i = shadowList.Count - 1; i >= Mathf.Max(0, shadowList.Count - removeCount); i--)
                {
                    shadowList[i].gameObject.SetActive(false);
                    shadowList.RemoveAt(i);
                }
            }
            shadowList = shadowList.OrderBy(obj => obj.GetInstanceID()).ToList();


        }
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

        List<Vector3> innerPoints = new List<Vector3>();
        List<Vector3> outerPoints = new List<Vector3>();


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


    private IEnumerator DecreaseStamina(float mT)
    {
        for (float eT = 0; eT < mT; eT += Time.deltaTime) 
        {
            sliderUI.value = 1f - eT / mT;
            yield return null;
        }
        sliderUI.value = 0;
        Destroy(gameObject);
    }
}
