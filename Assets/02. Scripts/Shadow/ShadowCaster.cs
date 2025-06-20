#nullable enable
#pragma warning disable CS8618
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.EventSystems;

public class ShadowCaster : MonoBehaviour, ICameraLookable, ISwitchable, ITouchable<MonoBehaviour>
{
    [Header("Light Attribute")]
    public float lightScale = 20f;                  // Light source scale (Sense distance is lightScale - minShadowScale)
    public float minShadowScale = 5f;
    public float activatedTime { get; private set; }

    public bool IsActivated { get; private set; }

    private Light2D light2D;
    private List<Shadow> shadowList = new List<Shadow>();       // Shadow Object Pool
    private PlayerCtrl player;
    private Coroutine staminaCor;

    private Collider2D[] curObstacles;
    private Collider2D[] lateObstacles;
    private List<List<Vector3>> shadowVertices = new List<List<Vector3>>();
    private Vector3 latePos;


    void OnEnable()
    {
        EnableCamera();
        Activate();
        staminaCor = StartCoroutine(DecreaseStamina(30f, 3f));
    }

    void Start()
    {
        light2D = gameObject.GetComponent<Light2D>();
    }

    void Update()
    {
        if (IsActivated)
        {
            Vector2 origin = transform.position;

            if(player != null && Vector2.Distance(origin, player.transform.position) > lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == true)
            {
                DisableCamera();
            }
            else if (player != null && Vector2.Distance(origin, player.transform.position) < lightScale && MainCineCam.Instance.targetTrList.Exists(tr => tr == transform) == false)
            {
                EnableCamera();
            }

            GenerateShadow(lightScale, minShadowScale, 1 << LayerMask.NameToLayer("Tile"));
            light2D.intensity = lightScale;
            light2D.pointLightOuterRadius = lightScale;
        }

        else
        {
            light2D.intensity = 0;
            light2D.pointLightOuterRadius = 0;
        }
    }

    void LateUpdate()
    {
        lateObstacles = curObstacles;
        latePos = transform.position;
    }

    void OnDisable()
    {
        Deactivate();
        DisableCamera();
    }

    private void GenerateShadow(float lS, float mS, int layer)
    {
        curObstacles = Physics2D.OverlapCircleAll(transform.position, lS - mS, layer);

        if (lateObstacles != null &&
            transform.position.Equals(latePos)
            && curObstacles.Length == lateObstacles.Length
            && curObstacles.All(o => lateObstacles.Any(l => o.transform.position.x == l.transform.position.x && o.transform.position.y == l.transform.position.y))
            && shadowList.Count >= curObstacles.Length
            && shadowVertices.Count >= curObstacles.Length)
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
        else if (collider is CompositeCollider2D composite)
        {
            for (int i = 0; i < composite.pathCount; i++)
            {
                Vector2[] pathPoints = new Vector2[composite.GetPathPointCount(i)];
                composite.GetPath(i, pathPoints);

                foreach (var point in pathPoints)
                {
                    vertices.Add(composite.transform.TransformPoint(point));
                }
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
            float hitDist = 1000f;

            RaycastHit2D hit = Physics2D.CircleCast(origin, 0.005f, dir, hitDist, 1 << LayerMask.NameToLayer("ScanTile"));
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
                    // Shadow Length
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


    private IEnumerator DecreaseStamina(float mT, float decreaseDT)
    {
        float eT = 0;
        while (true)
        {
            if (player.lanternValue.val > 0)
                player.lanternValue.val -= decreaseDT * Time.deltaTime;
            else
            {
                player.TakeDamage(null, decreaseDT * Time.deltaTime);
                eT += Time.deltaTime;
            }
            if(eT >= mT)
            {
                RetrieveLantern();
            }
            
            yield return null;
        }
    }

    public void Activate()
    {
        player = FindAnyObjectByType<PlayerCtrl>();
        activatedTime = Time.time;
        latePos = transform.position;

        
        var sw = FindAnyObjectByType<Switch>();
        if(sw != null)
        {
            sw.clients.Add(this);
            IsActivated = sw.IsActivated;
        }
        else
        {
            IsActivated = true;
        }
    }

    public void Deactivate()
    {
        foreach (var shadow in shadowList)
        {
            if (shadow != null)
            {
                shadow.gameObject.SetActive(false);
            }
        }
        shadowList.Clear();
        IsActivated = false;
    }

    public void HasTouched(PlayerCtrl player)
    {
        RetrieveLantern();
    }

    public void RetrieveLantern()
    {
        player.lantern = this;
        transform.SetParent(player.transform);
        StopCoroutine(staminaCor);
        staminaCor = null;
        gameObject.SetActive(false);
    }
}
