using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadowCaster : MonoBehaviour
{
    public float shadowDistance = 10f;      // maximum shadow length
    public Material shadowMaterial;         // shadow material

    [SerializeField] private List<Shadow> shadowList = new List<Shadow>();
    private float camWidth;
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        camWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
    }

    void Update()
    {
        GenerateShadow(shadowDistance);
    }

    //private void GenerateShadowMesh()
    //{
    //    List<Vector3> vertices = new List<Vector3>();
    //    List<int> triangles = new List<int>();

    //    Vector3 lightPos = transform.position;
    //    Collider2D[] obstacles = Physics2D.OverlapCircleAll(lightPos, camWidth, 1 << LayerMask.NameToLayer("Tile"));

    //    foreach (Collider2D obstacle in obstacles)
    //    {
    //        List<Vector3> objectVertices = GetColliderVertices(obstacle);

    //        foreach (Vector3 vertex in objectVertices)
    //        {
    //            Vector3 direction = (vertex - lightPos).normalized;
    //            Vector3 shadowPoint = vertex + direction * shadowDistance;

    //            int vertexIndex = vertices.Count;
    //            vertices.Add(transform.InverseTransformPoint(vertex));          // obstacle edges
    //            vertices.Add(transform.InverseTransformPoint(shadowPoint));     // shadow end point

    //            if (vertexIndex >= 2)
    //            {
    //                triangles.Add(vertexIndex - 2);
    //                triangles.Add(vertexIndex - 1);
    //                triangles.Add(vertexIndex);

    //                triangles.Add(vertexIndex - 1);
    //                triangles.Add(vertexIndex + 1);
    //                triangles.Add(vertexIndex);
    //            }
    //        }
    //    }

    //    mesh.Clear();
    //    mesh.vertices = vertices.ToArray();
    //    mesh.triangles = triangles.ToArray();
    //    mesh.RecalculateNormals();
    //}

    private void GenerateShadow(float shadowLen)
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, camWidth, 1 << LayerMask.NameToLayer("Tile"));

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

            List<Vector3> objectVertices = GetColliderVertices(obstacles[i]);
            List<Vector3> points = new List<Vector3>();
            foreach (var vertex in objectVertices)
            {
                Vector3 direction = (vertex - transform.position).normalized;
                Vector3 shadowPoint = vertex + direction * shadowLen;
                points.Add(transform.InverseTransformPoint(vertex));            // obstacle edges
                points.Add(transform.InverseTransformPoint(shadowPoint));       // shadow end point
            }
            shadow.SetCollision(points);
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

    private List<Vector3> GetColliderVertices(Collider2D collider)
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
}
