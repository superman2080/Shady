using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
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
        if (points.Count < 3) return;

        // 2D 점으로 변환
        List<Vector2> points2D = points.Select(p => new Vector2(p.x, p.y)).ToList();

        // PolygonCollider2D 설정
        col.SetPath(0, points2D.ToArray());

        // 삼각형 분할
        Triangulator triangulator = new Triangulator(points2D);
        int[] indices = triangulator.Triangulate();

        // 메시 생성
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDisable()
    {
        lightSource = null;
        col.isTrigger = false;
    }
}
