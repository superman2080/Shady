using UnityEngine;
using UnityEngine.Rendering;

public class ShadowManager: MonoBehaviour
{

    [SerializeField]
    private Material glDraw;
    private LightObject[] lights;
    private float camHeight, camWidth;

    void Start()
    {
        lights = FindObjectsByType<LightObject>(FindObjectsSortMode.InstanceID);
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = camHeight * Camera.main.aspect;

        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    private void OnPostRender()
    {
        float camLeft = transform.position.x - camWidth / 2;
        float camBottom = transform.position.y - camHeight / 2;

        GL.PushMatrix();
        glDraw.SetPass(0);
        GL.LoadOrtho();

        Collider2D[] col = Physics2D.OverlapBoxAll(transform.position, new Vector2(camWidth, camHeight), 0, LayerMask.GetMask("Tile"));
        foreach (var collision in col)
        {
            Tile tile = collision.GetComponent<Tile>();
            float left = (tile.left - camLeft) / camWidth;
            float top = (tile.top - camBottom) / camHeight;
            float right = (tile.right - camLeft) / camWidth;
            float bottom = (tile.bottom - camBottom) / camHeight;

            foreach (var light in lights)
            {
                if (light.transform.position.x <= tile.centerX && light.transform.position.y <= tile.centerY)
                    DrawShadow(left, bottom, right, top);

                if (light.transform.position.x <= tile.centerX && light.transform.position.y >= tile.centerY)
                    DrawShadow(left, top, right, bottom);

                if (light.transform.position.x >= tile.centerX && light.transform.position.y <= tile.centerY)
                    DrawShadow(right, bottom, left, top);

                if (light.transform.position.x >= tile.centerX && light.transform.position.y >= tile.centerY)
                    DrawShadow(right, top, left, bottom);
            }

        }
        GL.PopMatrix();
    }

    void DrawShadow(float x1, float y1, float x2, float y2)
    {
        float x = 0.5f, y = 0.5f;
        int len = 100;
        float projx1 = x2 + (x2 - x) * len;
        float projy1 = y1 + (y1 - y) * len;
        float projx2 = x1 + (x1 - x) * len;
        float projy2 = y2 + (y2 - y) * len;

        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.white);

        GL.Vertex(new Vector3(x1, y1, 0));
        GL.Vertex(new Vector3(x2, y1, 0));
        GL.Vertex(new Vector3(projx1, projy1, 0));



        GL.Vertex(new Vector3(x1, y1, 0));
        GL.Vertex(new Vector3(projx2, projy1, 0));
        GL.Vertex(new Vector3(projx1, projy1, 0));

        GL.Vertex(new Vector3(x1, y1, 0));
        GL.Vertex(new Vector3(x1, y2, 0));
        GL.Vertex(new Vector3(projx2, projy2, 0));

        GL.Vertex(new Vector3(x1, y1, 0));
        GL.Vertex(new Vector3(projx2, projy1, 0));
        GL.Vertex(new Vector3(projx2, projy2, 0));

        Debug.DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y1, 0), Color.white);
        Debug.DrawLine(new Vector3(projx1, projy1, 0), new Vector3(x2, y1, 0), Color.white);
        Debug.DrawLine(new Vector3(x1, y1, 0), new Vector3(projx1, projy1, 0), Color.white);

        GL.Vertex(new Vector3(1, 0, 0));
        GL.Vertex(new Vector3(1, 1, 0));
        GL.Vertex(new Vector3(0, 1, 0));

        GL.End();
    }

    private void OnDestroy()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }
}
