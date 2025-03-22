using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VertexShadowCasting2D : MonoBehaviour
{
    public Transform lightSource; // 광원의 Transform
    public LayerMask obstacleMask; // 장애물 레이어
    public float shadowDistance = 10f; // 그림자 최대 거리

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        GenerateShadowMesh();
    }

    void GenerateShadowMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 lightPos = lightSource.position;
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(lightPos, shadowDistance, obstacleMask);

        foreach (Collider2D obstacle in obstacles)
        {
            List<Vector3> objectVertices = GetColliderVertices(obstacle);

            foreach (Vector3 vertex in objectVertices)
            {
                Vector3 direction = (vertex - lightPos).normalized;
                Vector3 shadowPoint = vertex + direction * shadowDistance;

                int vertexIndex = vertices.Count;
                vertices.Add(transform.InverseTransformPoint(vertex)); // 장애물 꼭짓점
                vertices.Add(transform.InverseTransformPoint(shadowPoint)); // 그림자 끝점

                if (vertexIndex >= 2)
                {
                    triangles.Add(vertexIndex - 2);
                    triangles.Add(vertexIndex - 1);
                    triangles.Add(vertexIndex);

                    triangles.Add(vertexIndex - 1);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex);
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    List<Vector3> GetColliderVertices(Collider2D collider)
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
