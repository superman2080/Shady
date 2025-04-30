#nullable enable
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PolygonCollider2D))]
public class Shadow : MonoBehaviour
{
    [HideInInspector] public ShadowCaster lightSource;
    public PolygonCollider2D col { get; private set; }
    private MeshFilter meshFilter;
    private Mesh mesh;
    private void Awake()
    {
        col = gameObject.GetComponent<PolygonCollider2D>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
    }

    public void GenerateShadow(List<Vector3> points)
    {
        if (points.Count < 3) return; // 삼각형 이상만 그리기

        col.SetPath(0, points.Select(p => new Vector2(p.x, p.y)).ToArray());

        //중심점 계산
        Vector3 center = new Vector3(points.Average(p => p.x), points.Average(p => p.y));

        // 정점 목록 (중심점 + 꼭짓점)
        List<Vector3> vertices = new List<Vector3> { center };
        vertices.AddRange(points);

        // 삼각형 인덱스 생성 (팬 방식)
        List<int> triangles = new List<int>();
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0); // 중심점
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 마지막 꼭짓점 → 처음 꼭짓점 연결
        triangles.Add(0);
        triangles.Add(vertices.Count - 1);
        triangles.Add(1);

        // 메시 설정
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDisable()
    {
        lightSource = null;
    }
}
