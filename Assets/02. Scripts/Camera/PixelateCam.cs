using UnityEngine;

[ExecuteInEditMode]
public class PixelateCam: MonoBehaviour
{
    [Range(1, 100)] public int pixalate;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture result = RenderTexture.GetTemporary(source.width / pixalate, source.height / pixalate, 0, source.format);
        result.filterMode = FilterMode.Point;
        Graphics.Blit(source, result);
        Graphics.Blit(result, destination);
        RenderTexture.ReleaseTemporary(result);
    }
}
